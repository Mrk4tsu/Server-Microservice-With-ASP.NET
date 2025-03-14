using FN.Application.Systems.Token;
using FN.Utilities;
using FN.Utilities.Device;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.Token;
using MongoDB.Driver;

namespace FN.Application.Helper.Devices
{
    public class DeviceService : IDeviceService
    {
        private readonly IMongoCollection<UserDevice> _userDevicesCollection;
        private readonly ITokenService _tokenService;
        public DeviceService(IMongoDatabase database, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _userDevicesCollection = database.GetCollection<UserDevice>("UserDevices");
        }

        public async Task<ApiResult<bool>> RevokeDevice(TokenRequest request)
        {
            try
            {
                //1. Xóa Client khỏi danh sách thiết bị đã đăng ký
                var filter = Builders<UserDevice>.Filter.Eq(u => u.UserId, request.UserId);
                var update = Builders<UserDevice>.Update.PullFilter(u => u.Devices, d => d.ClientId == request.ClientId);

                await _userDevicesCollection.UpdateOneAsync(filter, update);
                //2. Xóa Refresh Token
                await _tokenService.RemoveRefreshToken(request);
                return new ApiSuccessResult<bool>();
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
            }
        }

        public async Task<ApiResult<bool>> RemoveAllDevice(int userId)
        {
            try
            {
                // 1. Xóa tất cả thiết bị đã đăng ký trong MongoDB
                var filter = Builders<UserDevice>.Filter.Eq(u => u.UserId, userId);
                var update = Builders<UserDevice>.Update.Set(u => u.Devices, new List<DeviceInfoDetail>());
                await _userDevicesCollection.UpdateOneAsync(filter, update);

                // 2. Xóa tất cả Refresh Tokens của user
                await _tokenService.RemoveAllTokensForUser(userId);
                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
            }
        }
        public async Task SaveDeviceInfo(TokenRequest request, string userAgent, string ipAddress)
        {
            var device = Commons.ParseUserAgent(userAgent);
            var deviceInfo = new DeviceInfoDetail
            {
                ClientId = request.ClientId,
                Browser = device.Browser,
                DeviceType = device.DeviceType,
                IPAddress = ipAddress,
                LastLogin = DateTime.Now,
                OS = device.OS,
            };
            // Lấy danh sách thiết bị hiện tại từ cơ sở dữ liệu
            var filter = Builders<UserDevice>.Filter.Eq(u => u.UserId, request.UserId);
            var userDevice = await _userDevicesCollection.Find(filter).FirstOrDefaultAsync();

            var deviceList = new DeviceLinkedList(10, request.UserId, _tokenService);

            if (userDevice != null)
            {
                // Thêm các thiết bị hiện tại vào Linked List
                foreach (var deviceItem in userDevice.Devices)
                {
                    await deviceList.Add(deviceItem);
                }
            }
            await deviceList.Add(deviceInfo);
            var update = Builders<UserDevice>.Update.Set(u => u.Devices, deviceList.ToList());
            await _userDevicesCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
        public async Task SaveDeviceInfo(TokenRequest request, DeviceInfoDetail deviceInfo, string ipAddress)
        {
            // Lấy danh sách thiết bị hiện tại từ cơ sở dữ liệu
            var filter = Builders<UserDevice>.Filter.Eq(u => u.UserId, request.UserId);
            var userDevice = await _userDevicesCollection.Find(filter).FirstOrDefaultAsync();

            var deviceList = new DeviceLinkedList(10, request.UserId, _tokenService);

            if (userDevice != null)
            {
                // Thêm các thiết bị hiện tại vào Linked List
                foreach (var deviceItem in userDevice.Devices)
                {
                    await deviceList.Add(deviceItem);
                }
            }
            await deviceList.Add(deviceInfo);
            var update = Builders<UserDevice>.Update.Set(u => u.Devices, deviceList.ToList());
            await _userDevicesCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }
        public async Task<ApiResult<List<DeviceInfoDetail>>> GetRegisteredDevices(int userId)
        {
            var userDevice = await _userDevicesCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();
            if (userDevice == null)
            {
                return new ApiErrorResult<List<DeviceInfoDetail>>("Không có CLJ hết");
            }
            return new ApiSuccessResult<List<DeviceInfoDetail>>(userDevice.Devices);
        }
        public async Task<bool> IsDeviceRegistered(TokenRequest request)
        {
            var filter = Builders<UserDevice>.Filter.And(
            Builders<UserDevice>.Filter.Eq(u => u.UserId, request.UserId),
            Builders<UserDevice>.Filter.ElemMatch(u => u.Devices,
                Builders<DeviceInfoDetail>.Filter.Eq(d => d.ClientId, request.ClientId)));
            return await _userDevicesCollection.Find(filter).AnyAsync();
        }
    }
}
