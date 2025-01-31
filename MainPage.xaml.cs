using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using BedChangeReminder.Views;
using Microsoft.Maui.Controls.Compatibility;
//using Microsoft.UI.Xaml.Data;

namespace BedChangeReminder
{
    public partial class MainPage : ContentPage
    {
        public MainViewModel ViewModel { get; }

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = viewModel;
        }

        // Open Popup when "Add Bed" is clicked
        private async void OnAddBedClicked(object sender, EventArgs e)
        {
            var popup = new BedPopup
            {
                OnBedSaved = async (name, date, action) =>
                {
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        // ✅ Ensure new Bed objects receive `ViewModel`
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            ViewModel.Beds.Add(new Bed(ViewModel)
                            {
                                Name = name,
                                LastChangeDate = date,
                                LastActionFlip = action == "Flipped"
                            });
                        });
                    }
                    else
                    {
                        var mainPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0].Page : null;
                        if (mainPage != null)
                        {
                            await mainPage.DisplayAlert("Error", "Bed name cannot be empty.", "OK");
                        }
                    }
                }
            };

            await this.ShowPopupAsync(popup);
        }

        // Open Popup when "Edit" is clicked
        private async void OnEditBedClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Bed selectedBed)
            {
                var popup = new BedPopup
                {
                    OnBedSaved = async (name, date, action) =>
                    {
                        // Use MainThread.BeginInvokeOnMainThread to update UI
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            selectedBed.Name = name;
                            selectedBed.LastChangeDate = date;
                            selectedBed.LastActionFlip = action == "Flipped";

                            ViewModel.RefreshBeds();
                        });

                        // Get the main page safely before using it
                        var mainPage = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0].Page : null;
                        if (mainPage != null)
                        {
                            await mainPage.DisplayAlert("Success", "Bed details updated successfully.", "OK");
                        }
                    }
                };

                await this.ShowPopupAsync(popup); // ✅ Ensure ShowPopup is awaited
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (MainGrid != null)
            {
                MainGrid.InvalidateMeasure(); // 🔹 Forces re-measurement
                MainGrid.Handler?.UpdateValue(nameof(HeightRequest));       // 🔹 Forces re-layout
            }
            this.ForceLayout(); // ✅ Forces UI to refresh and apply bindings
            OnPropertyChanged(nameof(BindingContext));
        }
    }
}
