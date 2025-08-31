using System;
using System.Threading.Tasks;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace BedChangeReminder.Services
{
    public interface IBedNotificationService
    {
        Task<bool> RequestPermissions();
        Task ScheduleNotification(int bedId, string bedName, DateTime dueDate);
        Task CancelNotification(int bedId);
        Task CancelAllNotifications();
        Task ShowImmediateTestAsync(); // helper for instant test
    }

    public class BedNotificationService : IBedNotificationService
    {
        private const string ChannelId = "bed_reminders";
        private static bool _channelCreated;

        public BedNotificationService() { }

        public async Task<bool> RequestPermissions()
            => await LocalNotificationCenter.Current.RequestNotificationPermission();

//        private async Task EnsureChannelAsync()
//        {
//#if ANDROID
//            if (_channelCreated) return;

//            await LocalNotificationCenter.Current.CreateChannelAsync(new NotificationChannelRequest
//            {
//                Id = ChannelId,
//                Name = "Bed Change Reminders",
//                Description = "Due reminders and test notifications",
//                Importance = AndroidImportance.High
//            });

//            _channelCreated = true;
//#endif
//        }

        public async Task ScheduleNotification(int bedId, string bedName, DateTime dueDate)
        {
            // Cancel any existing notification for this bed
            await CancelNotification(bedId);

            // Your original scheduling logic (9am the day before; else 9am day-of; else tomorrow 9am)
            var notificationTime = dueDate.Date.AddDays(-1).AddHours(9);
            if (notificationTime <= DateTime.Now)
            {
                notificationTime = dueDate.Date.AddHours(9);
                if (notificationTime <= DateTime.Now)
                    notificationTime = DateTime.Now.Date.AddDays(1).AddHours(9);
            }

            //await EnsureChannelAsync();

            var request = new NotificationRequest
            {
                NotificationId = bedId,
                Title = "🛏️ Bed Change Reminder",
                Subtitle = $"{bedName} is due for changing",
                Description = $"Time to change the sheets for {bedName}!",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule { NotifyTime = notificationTime },
#if ANDROID
                Android = new AndroidOptions
                {
                    ChannelId = ChannelId,
                    Priority = AndroidPriority.High
                }
#endif
            };

            await LocalNotificationCenter.Current.Show(request);
        }

        public async Task ShowImmediateTestAsync()
        {
            await RequestPermissions();
            //await EnsureChannelAsync();

            var req = new NotificationRequest
            {
                NotificationId = 9999,
                Title = "Bed Change Reminder (Test)",
                Description = "If you see this, notifications work.",
#if ANDROID
                Android = new AndroidOptions
                {
                    ChannelId = ChannelId,
                    Priority = AndroidPriority.High
                }
#endif
            };

            await LocalNotificationCenter.Current.Show(req);
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
