using CommunityToolkit.Maui.Views;
using BedChangeReminder.Models;
using BedChangeReminder.ViewModels;

namespace BedChangeReminder.Views.Popups
{
    public partial class EditBedPopup : Popup
    {
        public EditBedPopup(Bed selectedBed, Action<Bed> onConfirm)
        {
            InitializeComponent();
            BindingContext = new EditBedPopupViewModel(selectedBed, onConfirm, () => { this.Close(); });
        }
    }
}
