using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using BedChangeReminder.Models;
using BedChangeReminder.Views.Popups;
using BedChangeReminder.Services;

namespace BedChangeReminder.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly BedDatabase _bedDatabase;
        private readonly IBedNotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<Bed> beds = new();

        [ObservableProperty]
        private bool isRefreshing;

        public string BedsStatusSummary
        {
            get
            {
                if (Beds.Count == 0) return "No beds tracked ";

                var overdue = Beds.Count(b => b.IsOverdue);
                var dueToday = Beds.Count(b => b.IsDueToday);
                var dueSoon = Beds.Count(b => b.IsDueSoon);

                var parts = new List<string>();
                if (overdue > 0) parts.Add($"{overdue} overdue");
                if (dueToday > 0) parts.Add($"{dueToday} due today");
                if (dueSoon > 0) parts.Add($"{dueSoon} due soon");

                return parts.Count > 0 ? string.Join(", ", parts) + " " : $"{Beds.Count} beds tracked, all up to date ";
            }
        }

        public MainViewModel(BedDatabase bedDatabase, IBedNotificationService notificationService)
        {
            _bedDatabase = bedDatabase;
            _notificationService = notificationService;

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadBedsAsync();
            await _notificationService.RequestPermissions();
        }

        [RelayCommand]
        private async Task LoadBedsAsync()
        {
            try
            {
                var bedList = await _bedDatabase.GetBedsAsync();
                Beds = new ObservableCollection<Bed>(bedList.OrderBy(b => b.NextChangeDate));
                OnPropertyChanged(nameof(BedsStatusSummary));
            }
            catch (Exception ex)
            {
                // TODO: Handle error properly
                System.Diagnostics.Debug.WriteLine($"Error loading beds: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await LoadBedsAsync();
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task AddBed()
        {
            var popup = new AddBedPopup(async (newBed) =>
            {
                if (newBed != null)
                {
                    await _bedDatabase.SaveBedAsync(newBed);
                    Beds.Add(newBed);
                    await _notificationService.ScheduleNotification(newBed.Id, newBed.Name, newBed.NextChangeDate);
                    OnPropertyChanged(nameof(BedsStatusSummary));
                }
            });

            await Shell.Current.ShowPopupAsync(popup);
        }

        [RelayCommand]
        private async Task EditBed(Bed selectedBed)
        {
            if (selectedBed == null) return;

            var popup = new EditBedPopup(selectedBed, async (updatedBed) =>
            {
                if (updatedBed != null)
                {
                    await _bedDatabase.SaveBedAsync(updatedBed);
                    var existingBed = Beds.FirstOrDefault(b => b.Id == updatedBed.Id);
                    if (existingBed != null)
                    {
                        existingBed.Name = updatedBed.Name;
                        existingBed.Frequency = updatedBed.Frequency;
                        existingBed.LastChangeDate = updatedBed.LastChangeDate;
                        existingBed.LastAction = updatedBed.LastAction;

                        // Refresh notifications
                        await _notificationService.ScheduleNotification(existingBed.Id, existingBed.Name, existingBed.NextChangeDate);
                        OnPropertyChanged(nameof(BedsStatusSummary));
                    }
                }
            });

            await Shell.Current.ShowPopupAsync(popup);
        }

        [RelayCommand]
        private async Task ChangeBed(Bed selectedBed)
        {
            if (selectedBed == null) return;

            var popup = new ChangeBedPopup(selectedBed, async (updatedBed) =>
            {
                if (updatedBed != null)
                {
                    // Update the bed with new change date and action
                    updatedBed.LastChangeDate = DateTime.Today;

                    await _bedDatabase.SaveBedAsync(updatedBed);
                    var existingBed = Beds.FirstOrDefault(b => b.Id == updatedBed.Id);
                    if (existingBed != null)
                    {
                        existingBed.LastChangeDate = updatedBed.LastChangeDate;
                        existingBed.LastAction = updatedBed.LastAction;

                        // Refresh notifications
                        await _notificationService.ScheduleNotification(existingBed.Id, existingBed.Name, existingBed.NextChangeDate);
                        OnPropertyChanged(nameof(BedsStatusSummary));
                    }
                }
            });

            await Shell.Current.ShowPopupAsync(popup);
        }

        [RelayCommand]
        private async Task DeleteBed(Bed selectedBed)
        {
            if (selectedBed == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Bed",
                $"Are you sure you want to delete '{selectedBed.Name}'?",
                "Delete",
                "Cancel");

            if (confirm)
            {
                await _bedDatabase.DeleteBedAsync(selectedBed);
                await _notificationService.CancelNotification(selectedBed.Id);
                Beds.Remove(selectedBed);
                OnPropertyChanged(nameof(BedsStatusSummary));
            }
        }

        [RelayCommand]
        private async Task TestNotification()
        {
            try
            {
                // Request permissions first
                var hasPermission = await _notificationService.RequestPermissions();
                if (!hasPermission)
                {
                    await Shell.Current.DisplayAlert("Permission Required",
                        "Please enable notifications in your device settings", "OK");
                    return;
                }

                // Schedule a test notification for 10 seconds from now
                await _notificationService.ScheduleNotification(
                    999, // Test notification ID
                    "Test Bed",
                    DateTime.Now.AddSeconds(10) // This will trigger the "tomorrow at 9 AM" fallback
                );

                await Shell.Current.DisplayAlert("Test Scheduled",
                    "Test notification scheduled! It should appear in about 10 seconds.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error",
                    $"Failed to schedule notification: {ex.Message}", "OK");
            }
        }
    }
}