using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BedChangeReminder
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<Bed> Beds { get; } = [];

        public MainViewModel()
        {
            //Beds.Add(new Bed(this) { Name = "Master Bed", LastChangeDate = DateTime.Today, Frequency = 7, LastActionFlip = false });
        }

        // Command for "Add Bed" Button
        [RelayCommand]
        public async Task AddBed()
        {
            var shell = Shell.Current;
            string name;
            string freq;
            if (shell != null)
            {
                name = await shell.DisplayPromptAsync("Add Bed", "Enter bed name:") ?? string.Empty;
                freq = await shell.DisplayPromptAsync("Change Frequency", "Enter days between changes:", keyboard: Keyboard.Numeric) ?? string.Empty;
            }
            else
            {
                name = string.Empty;
                freq = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(name) && int.TryParse(freq, out int frequency))
            {
                Beds.Add(new Bed(this) { Name = name, Frequency = frequency, LastChangeDate = DateTime.Today, LastActionFlip = false });
            }
        }

        // Command for "Log Change" Button
        [RelayCommand]
        public void LogBedChange(Bed bed)
        {
            if (bed == null) return;

            bed.LastActionFlip = !bed.LastActionFlip;
            bed.LastChangeDate = DateTime.Today;

            OnPropertyChanged(nameof(Beds)); // Notify UI
        }

        [RelayCommand]
        public static void UpdateBed(Bed bed)
        {
            if (bed == null) return;

            var popup = new Views.BedPopup
            {
                OnBedSaved = (name, date, action) =>
                {
                    bed.Name = name;
                    bed.LastChangeDate = date;
                    bed.LastActionFlip = action == "Flipped";
                }
            };

            var currentPage = Shell.Current?.CurrentPage;
            currentPage?.ShowPopup(popup);
        }

        public void RefreshBeds()
        {
            OnPropertyChanged(nameof(Beds)); // 🔹 Forces UI to update
        }
    }
}
