using BedChangeReminder.Models;
using CommunityToolkit.Maui.Views;
using BedChangeReminder.ViewModels;

namespace BedChangeReminder.Views.Popups
{
    public partial class ChangeBedPopup : Popup
    {
        public ChangeBedPopup(Bed selectedBed, Action<Bed> onConfirm)
        {
            InitializeComponent();
            BindingContext = new ChangeBedPopupViewModel(selectedBed, onConfirm, () => { this.Close(); });
        }
    }
}