using BedChangeReminder;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BedChangeReminder.Models;

namespace BedChangeReminder.ViewModels
{
    public partial class AddBedPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private Bed newBed;

        [ObservableProperty]
        private BedAction tempSelectedAction;

        [ObservableProperty]
        private bool changedMattressProtector;

        [ObservableProperty]
        private bool changedPillowLiner;

        private readonly Action<Bed> _onConfirm;
        private readonly Action _onCancel;

        public AddBedPopupViewModel(Action<Bed> onConfirm, Action onCancel)
        {
            NewBed = new Bed
            {
                Name = "Main Bed",
                Frequency = 14,
                LastChangeDate = DateTime.Today,
                LastAction = BedAction.None
            };

            TempSelectedAction = NewBed.LastAction;
            ChangedMattressProtector = false;
            ChangedPillowLiner = false;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
        }

        [RelayCommand]
        private void SelectNone() => TempSelectedAction = BedAction.None;

        [RelayCommand]
        private void SelectFlip() => TempSelectedAction = BedAction.Flip;

        [RelayCommand]
        private void SelectRotate() => TempSelectedAction = BedAction.Rotate;

        [RelayCommand]
        private void Confirm()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(NewBed.Name))
            {
                // TODO: Show validation error
                return;
            }

            if (NewBed.Frequency <= 0)
            {
                // TODO: Show validation error  
                return;
            }

            NewBed.LastAction = TempSelectedAction;
            NewBed.LastMattressProtectorChangeDate = ChangedMattressProtector ? NewBed.LastChangeDate : null;
            NewBed.LastPillowLinerChangeDate = ChangedPillowLiner ? NewBed.LastChangeDate : null;
            _onConfirm?.Invoke(NewBed);
            _onCancel?.Invoke(); // Close popup
        }

        [RelayCommand]
        private void Cancel()
        {
            _onCancel?.Invoke();
        }
    }
}