using FN.Utilities.Device;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.Token;

namespace FN.Application.Helper.Devices
{
    public interface IDeviceService
    {
        Task<ApiResult<bool>> RevokeDevice(TokenRequest request);
        Task<ApiResult<bool>> RemoveAllDevice(int userId);
        Task SaveDeviceInfo(TokenRequest request, string userAgent, string ipAddress);
        Task SaveDeviceInfo(TokenRequest request, DeviceInfoDetail deviceInfo, string ipAddress);
        Task<ApiResult<List<DeviceInfoDetail>>> GetRegisteredDevices(int userId);
        Task<bool> IsDeviceRegistered(TokenRequest request);
    }
}
