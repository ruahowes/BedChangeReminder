using CommunityToolkit.Maui.Views;
using BedChangeReminder.Models;
using BedChangeReminder.ViewModels;

namespace BedChangeReminder.Views.Popups
{
    public partial class AddBedPopup : Popup
    {
        public AddBedPopup(Action<Bed> onConfirm)
        {
            InitializeComponent();
            BindingContext = new AddBedPopupViewModel(onConfirm, () => { this.Close(); });
        }
    }
}

