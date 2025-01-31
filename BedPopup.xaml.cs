using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System;

namespace BedChangeReminder.Views
{
    public partial class BedPopup : Popup
    {
        public Action<string, DateTime, string, int>? OnBedSaved;

        public BedPopup(Bed? existingBed = null)
        {
            InitializeComponent();

            if (existingBed != null)
            {
                BedNameEntry.Text = existingBed.Name;
                FrequencyEntry.Text = existingBed.Frequency.ToString();
                ChangeDatePicker.Date = existingBed.LastChangeDate;
                FlipRotatePicker.SelectedItem = existingBed.LastAction;
            }
        }

        private void OnConfirmClicked(object sender, EventArgs e)
        {
            if (OnBedSaved != null) // Ensure it’s not null
            {
                string bedName = BedNameEntry.Text ?? string.Empty;
                DateTime changeDate = ChangeDatePicker.Date;
                string flipOrRotate = FlipRotatePicker.SelectedItem?.ToString() ?? "Flipped";
                int frequency = int.TryParse(FrequencyEntry.Text, out int freqValue) ? freqValue : 7; // Default to 7 days

                OnBedSaved.Invoke(bedName, changeDate, flipOrRotate, frequency);
                Close();
            }
            else
            {
                Console.WriteLine("OnBedSaved delegate is null!");
            }
        }
    }
}