using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System;

namespace BedChangeReminder.Views
{
    public partial class BedPopup : Popup
    {
        public Action<string, DateTime, string>? OnBedSaved;

        public BedPopup()
        {
            InitializeComponent();
        }

        private void OnConfirmClicked(object sender, EventArgs e)
        {
            if (OnBedSaved != null) // Ensure it’s not null
            {
                string bedName = BedNameEntry.Text ?? string.Empty;
                DateTime changeDate = ChangeDatePicker.Date;
                string flipOrRotate = FlipRotatePicker.SelectedItem?.ToString() ?? "Flipped";

                OnBedSaved.Invoke(bedName, changeDate, flipOrRotate);
                Close();
            }
            else
            {
                Console.WriteLine("OnBedSaved delegate is null!");
            }
        }
    }
}