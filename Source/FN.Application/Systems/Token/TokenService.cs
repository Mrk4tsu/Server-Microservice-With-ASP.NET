using FN.Application.Systems.Redis;
using FN.DataAccess.Entities;
using FN.ViewModel.Systems.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FN.Application.Systems.Token
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IRedisService _redisService;
        private readonly IConfiguration _configuration;

        private readonly SymmetricSecurityKey _securityKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly double _expiryMinutes;
        public TokenService(IConfiguration configuration,
            UserManager<AppUser> userManager,
            IRedisService redisService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _redisService = redisService;

            var keyString = _configuration["Tokens:Key"] ?? throw new ArgumentNullException("Tokens:Key", "Token key must be configured.");
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            _issuer = _configuration["Tokens:Issuer"]!;
            _audience = _configuration["Tokens:Audience"]!;
            if (!double.TryParse(_configuration["Tokens:AccessTokenExpiryMinutes"], out _expiryMinutes))
            {
                _expiryMinutes = 30;
            }
        }
        public async Task<string> GenerateAccessToken(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim("Avatar", user.Avatar),
                new Claim("Role", string.Join(';', roles))
            };

            var token = new JwtSecurityToken(
              issuer: _issuer,
              audience: _audience,
              claims: claims,
              expires: DateTime.Now.AddMinutes(_expiryMinutes),
              signingCredentials: _signingCredentials
          );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken() => Guid.NewGuid().ToString();
        public async Task<string?> GetRefreshToken(TokenRequest request)
        {
            var key = $"auth:{request.UserId}:refresh_token:{request.ClientId}";
            return await _redisService.GetValue<string>(key);
        }
        public async Task RemoveAllTokensForUser(int userId)
        {
            var pattern = $"auth:{userId}:*";
            var keys =  _redisService.GetKeysByPattern(pattern);
            foreach (var key in keys)
            {
                await _redisService.RemoveValue(key);
            }
        }
        public async Task RemoveRefreshToken(TokenRequest request)
        {
            var key = $"auth:{request.UserId}:refresh_token:{request.ClientId}";
            if (await _redisService.KeyExist(key))
                await _redisService.RemoveValue(key);
        }
        public async Task SaveRefreshToken(string refreshToken, TokenRequest request, TimeSpan expiry)
        {
            var key = $"auth:{request.UserId}:refresh_token:{request.ClientId}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            };
            await _redisService.SetValue(key, refreshToken, expiry);
        }     
    }
}
