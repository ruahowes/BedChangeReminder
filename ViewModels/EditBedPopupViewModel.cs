using BedChangeReminder.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BedChangeReminder.ViewModels
{
    public partial class EditBedPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        private Bed editableBed;

        [ObservableProperty]
        private BedAction tempSelectedAction;

        [ObservableProperty]
        private bool changedMattressProtector;

        [ObservableProperty]
        private bool changedPillowLiner;

        public string MattressProtectorStatusText { get; }
        public string PillowLinerStatusText { get; }

        private readonly Action<Bed> _onConfirm;
        private readonly Action _onCancel;

        public EditBedPopupViewModel(Bed bed, Action<Bed> onConfirm, Action onCancel)
        {
            EditableBed = new Bed
            {
                Id = bed.Id,
                Name = bed.Name,
                Frequency = bed.Frequency,
                LastChangeDate = bed.LastChangeDate,
                LastAction = bed.LastAction,
                LastMattressProtectorChangeDate = bed.LastMattressProtectorChangeDate,
                LastPillowLinerChangeDate = bed.LastPillowLinerChangeDate
            };

            TempSelectedAction = bed.LastAction;
            ChangedMattressProtector = false;
            ChangedPillowLiner = false;
            MattressProtectorStatusText = $"Currently: {bed.MattressProtectorSummaryText}";
            PillowLinerStatusText = $"Currently: {bed.PillowLinerSummaryText}";
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
            if (string.IsNullOrWhiteSpace(EditableBed.Name) || EditableBed.Frequency <= 0)
            {
                // TODO: Show validation error
                return;
            }

            EditableBed.LastAction = TempSelectedAction;

            if (ChangedMattressProtector)
                EditableBed.LastMattressProtectorChangeDate = EditableBed.LastChangeDate;

            if (ChangedPillowLiner)
                EditableBed.LastPillowLinerChangeDate = EditableBed.LastChangeDate;

            _onConfirm?.Invoke(EditableBed);
            _onCancel?.Invoke(); // Close popup
        }

        [RelayCommand]
        private void Cancel() => _onCancel?.Invoke();
    }
}