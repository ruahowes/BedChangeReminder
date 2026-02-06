using BedChangeReminder.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BedChangeReminder.ViewModels
{
    public partial class ChangeBedPopupViewModel : ObservableObject
    {
        private readonly Bed _originalBed;
        private readonly Action<Bed> _onConfirm;
        private readonly Action _onCancel;

        [ObservableProperty]
        private string bedName;

        [ObservableProperty]
        private BedAction selectedAction;

        [ObservableProperty]
        private DateTime changeDate;

        [ObservableProperty]
        private bool changedMattressProtector;

        [ObservableProperty]
        private bool changedPillowLiner;

        public string CurrentActionText =>
            $"Last action: {_originalBed.LastActionText} on {_originalBed.LastChangeDate:dd MMM yyyy}";

        public string RecommendedActionText =>
            $"Recommended next action: {_originalBed.NextActionText}";

        public string PillowLinerStatusText =>
            $"Pillow liners: {_originalBed.PillowLinerSummaryText}";

        public string MattressProtectorStatusText =>
            $"Mattress protector: {_originalBed.MattressProtectorSummaryText}";

        public ChangeBedPopupViewModel(Bed bed, Action<Bed> onConfirm, Action onCancel)
        {
            _originalBed = bed;
            _onConfirm = onConfirm;
            _onCancel = onCancel;

            BedName = bed.Name;
            ChangeDate = DateTime.Today;
            ChangedMattressProtector = false;
            ChangedPillowLiner = false;

            // Pre-select the recommended action
            SelectedAction = bed.LastAction switch
            {
                BedAction.None => BedAction.Flip, // If no previous action, suggest flip
                BedAction.Flip => BedAction.Rotate, // If last was flip, suggest rotate
                BedAction.Rotate => BedAction.Flip, // If last was rotate, suggest flip
                _ => BedAction.Flip
            };
        }

        [RelayCommand]
        private void SelectNone() => SelectedAction = BedAction.None;

        [RelayCommand]
        private void SelectFlip() => SelectedAction = BedAction.Flip;

        [RelayCommand]
        private void SelectRotate() => SelectedAction = BedAction.Rotate;

        [RelayCommand]
        private void Confirm()
        {
            // Create updated bed with new values
            var updatedBed = new Bed
            {
                Id = _originalBed.Id,
                Name = _originalBed.Name,
                Frequency = _originalBed.Frequency,
                LastChangeDate = ChangeDate,
                LastAction = SelectedAction == BedAction.None ? _originalBed.LastAction : SelectedAction,
                LastMattressProtectorChangeDate = ChangedMattressProtector ? ChangeDate : _originalBed.LastMattressProtectorChangeDate,
                LastPillowLinerChangeDate = ChangedPillowLiner ? ChangeDate : _originalBed.LastPillowLinerChangeDate
            };

            _onConfirm?.Invoke(updatedBed);
            _onCancel?.Invoke(); // Close popup
        }

        [RelayCommand]
        private void Cancel()
        {
            _onCancel?.Invoke();
        }
    }
}