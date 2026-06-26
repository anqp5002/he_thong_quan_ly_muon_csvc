using System.Collections.Generic;
using System.Threading.Tasks;
using CSVC_PTIT.Data.Entities;

namespace CSVC_PTIT.Core.Interfaces;

public interface INotificationService
{
    Task CreateNotificationAsync(int userId, string title, string message);
    Task<List<Notification>> GetUnreadNotificationsAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
}
