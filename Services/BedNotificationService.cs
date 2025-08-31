using Plugin.LocalNotification;

namespace BedChangeReminder.Services
{
    // Create a simple interface just for our app's needs
    public interface IBedNotificationService
    {
        Task<bool> RequestPermissions();
        Task ScheduleNotification(int bedId, string bedName, DateTime dueDate);
        Task CancelNotification(int bedId);
        Task CancelAllNotifications();
    }

    public class BedNotificationService : IBedNotificationService
    {
        public async Task<bool> RequestPermissions()
        {
            return await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        public async Task ScheduleNotification(int bedId, string bedName, DateTime dueDate)
        {
            // Cancel any existing notification for this bed
            await CancelNotification(bedId);

            // Schedule notification for day before due date at 9 AM
            var notificationTime = dueDate.Date.AddDays(-1).AddHours(9);

            // If notification time has already passed, schedule for due date instead
            if (notificationTime <= DateTime.Now)
            {
                notificationTime = dueDate.Date.AddHours(9);

                // If that's also passed, schedule for tomorrow at 9 AM
                if (notificationTime <= DateTime.Now)
                {
                    notificationTime = DateTime.Now.Date.AddDays(1).AddHours(9);
                }
            }

            var request = new NotificationRequest
            {
                NotificationId = bedId, // Use bed ID as notification ID
                Title = "🛏️ Bed Change Reminder",
                Subtitle = $"{bedName} is due for changing",
                Description = $"Time to change the sheets for {bedName}!",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notificationTime
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }

        public async Task CancelNotification(int bedId)
        {
            LocalNotificationCenter.Current.Cancel(bedId);
            await Task.CompletedTask;
        }

        public async Task CancelAllNotifications()
        {
            LocalNotificationCenter.Current.CancelAll();
            await Task.CompletedTask;
        }
    }
}