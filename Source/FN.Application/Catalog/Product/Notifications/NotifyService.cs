using CloudinaryDotNet.Actions;
using FN.DataAccess.Entities;
using FN.Utilities.Device;
using FN.ViewModel.Helper.API;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace FN.Application.Catalog.Product.Notifications
{
    public interface INotifyService
    {
        Task SaveNotify(string u, Notification notification);
        Task<ApiResult<List<UserNotification>>> ListNotify(int userId);
    }
    public class NotifyService : INotifyService
    {
        private readonly IMongoCollection<UserNotification> _notifyCollection;
        public NotifyService(IMongoDatabase database)
        {
            _notifyCollection = database.GetCollection<UserNotification>("Notifications");
        }
        public async Task<ApiResult<List<UserNotification>>> ListNotify(int userId)
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            // Filter notifications for the user and within the last 7 days
            var filter = Builders<UserNotification>.Filter.And(
                Builders<UserNotification>.Filter.Eq(n => n.UserId, userId.ToString()),
                Builders<UserNotification>.Filter.ElemMatch(n => n.Notifications, notify => notify.Time >= sevenDaysAgo)
            );

            var result = await _notifyCollection.Find(filter).ToListAsync();
            var sortedResult = result
                .Select(userNotification =>
                {
                    userNotification.Notifications = userNotification.Notifications
                        .Where(notify => notify.Time >= sevenDaysAgo)
                        .OrderByDescending(notify => notify.Time)
                        .ToList();
                    return userNotification;
                })
                .OrderByDescending(userNotification => userNotification.Notifications.FirstOrDefault()?.Time)
                .ToList();

            return new ApiSuccessResult<List<UserNotification>>(sortedResult);
        }
        public async Task SaveNotify(string userId, Notification notification)
        {
            // Kiểm tra tài liệu có tồn tại không
            var filter = Builders<UserNotification>.Filter.Eq(u => u.UserId, userId);
            var existingDocument = await _notifyCollection.Find(filter).FirstOrDefaultAsync();

            if (existingDocument == null)
            {
                // Nếu không tồn tại, tạo mới tài liệu
                var newDocument = new UserNotification
                {
                    UserId = userId,
                    Notifications = new List<Notification> { notification }
                };
                await _notifyCollection.InsertOneAsync(newDocument);
            }
            else
            {
                // Nếu tồn tại, cập nhật tài liệu
                var update = Builders<UserNotification>.Update.Push(u => u.Notifications, notification);
                await _notifyCollection.UpdateOneAsync(filter, update);
            }
        }
    }
}
