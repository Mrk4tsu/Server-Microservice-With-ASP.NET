using FN.Application.Helper.Devices;
using FN.Application.Helper.Mail;
using FN.Application.Systems.Redis;
using FN.Application.Systems.User;
using FN.Utilities;
using FN.Utilities.Device;
using FN.ViewModel.Systems.User;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace FN.EmailService
{
    public class MailSubscriber : BackgroundService
    {
        private readonly IMailService _mailService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRedisService _redisService;
        private readonly IConfiguration _configuration;
        public MailSubscriber(IMailService mailService,
            IServiceProvider serviceProvider,
            IRedisService redisService,
            IConfiguration configuration)
        {
            _mailService = mailService;
            _serviceProvider = serviceProvider;
            _redisService = redisService;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _redisService.Subscribe(SystemConstant.MESSAGE_PATTERN_EVENT, async (channel, message) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IMailService>();
                var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();

                var baseDomain = _configuration["BaseDomain"];
                switch ((string)channel!)
                {
                    case SystemConstant.MESSAGE_REGISTER_EVENT:
                        var user = JsonSerializer.Deserialize<RegisterResponse>(message!);
                        var vars = new Dictionary<string, object>()
                        {
                            {"pusername", user?.FullName!}
                        };
                        if (user!.Status)
                            await _mailService.SendMail(user!.Email, $"Chào mừng {user.FullName} đến MrKatsu Shop!", SystemConstant.TEMPLATE_WELCOME_ID, vars);
                        break;
                    case SystemConstant.MESSAGE_LOGIN_EVENT:
                        var userLogin = JsonSerializer.Deserialize<LoginResponse>(message!);

                        if (!await deviceService.IsDeviceRegistered(userLogin!.Token))
                        {
                            var deviceInfo = Commons.ParseUserAgent(userLogin!.UserAgent);
                            var device = new DeviceInfoDetail
                            {
                                ClientId = userLogin.Token.ClientId,
                                Browser = deviceInfo.Browser,
                                DeviceType = deviceInfo.DeviceType,
                                IPAddress = userLogin.IpAddress,
                                LastLogin = DateTime.Now,
                                OS = deviceInfo.OS
                            };
                            var variables = new JObject
                            {
                                {"pbrowser", device.Browser},
                                {"pos", device.OS},
                                {"ptime", device.LastLogin.ToString("hh:mm dd/MM/yyyy")},
                                {"puser", userLogin.Username},
                                {"pip", device.IPAddress}
                            };
                            await deviceService.SaveDeviceInfo(userLogin.Token, device, device.IPAddress);
                            if (!await _mailService.IsJustSendMail(userLogin.Token.UserId))
                                await _mailService.SendMail(userLogin!.Email, $"Cảnh báo bảo mật cho {userLogin.Username}", SystemConstant.TEMPLATE_WARNING_ID, variables);
                            await _redisService.SetValue($"auth:{userLogin.Token.UserId}:just_send_mail", userLogin.Username, TimeSpan.FromMinutes(30));
                        }
                        break;
                    case SystemConstant.MESSAGE_UPDATE_EMAIL_EVENT:
                        var req = JsonSerializer.Deserialize<UpdateEmailResponse>(message!);

                        var link = UrlCallback(req!.UserId, req.NewEmail, req.Token, baseDomain ?? "https://mrkatsu.io.vn");
                        var obj = new JObject
                        {
                            {"plink", link}
                        };
                        await _mailService.SendMail(req!.NewEmail, $"Xác nhận thay đổi email", SystemConstant.TEMPLATE_UPDATE_MAIL_ID, obj);
                        break;
                    case SystemConstant.MESSAGE_FORGOT_PASSWORD_EVENT:
                        var request = JsonSerializer.Deserialize<UserMailTokenResponse>(message!);
                        if (request != null)
                        {
                            var url = UrlCallback(request.Token, request.Username, baseDomain ?? "https://mrkatsu.io.vn", "confirm-password");
                            var objects = new JObject
                            {
                                {"plink", url}
                            };
                            await _mailService.SendMail(request.Email, $"Xác nhận khôi phục mật khẩu", SystemConstant.TEMPLATE_RESET_PASSWORD_ID, objects);
                        }
                        break;
                    case SystemConstant.MESSAGE_CONFIRM_ACCOUNT_EVENT:
                        var confirmRequest = JsonSerializer.Deserialize<UserMailTokenResponse>(message!);
                        if(confirmRequest != null)
                        {
                            var url = UrlCallback(confirmRequest.Token, confirmRequest.Username, baseDomain ?? "https://mrkatsu.io.vn", "verify");
                            var objects = new JObject
                            {
                                {"plink", url}
                            };
                            await _mailService.SendMail(confirmRequest.Email, $"Xác nhận tài khoản", SystemConstant.TEMPLATE_CONFIRM_ACCOUNT, objects);
                        }   
                        break;
                    default:
                        break;
                }

            });

            // Ensure the background service runs continuously
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private string UrlCallback(int userId, string token, string domain, string? newEmail = null)
        {
            var encodedToken = WebUtility.UrlEncode(token);
            return $"{domain}/confirm-email?userId={userId}&newEmail={newEmail}&token={encodedToken}";
        }
        private string UrlCallback(string token, string username, string domain, string page)
        {
            var encodedToken = WebUtility.UrlEncode(token);
            return $"{domain}/{page}?username={username}&token={encodedToken}";
        }
    }
}
