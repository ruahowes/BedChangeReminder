using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BedChangeReminder
{
    public partial class Bed : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ObservableProperty] private string? name;
        [ObservableProperty] private int frequency; // Days
        [ObservableProperty] private DateTime lastChangeDate;
        [ObservableProperty] private bool lastActionFlip; // True = Flip, False = Rotate

        public DateTime NextChangeDate => LastChangeDate.AddDays(Frequency);

        public string LastAction => LastActionFlip ? "Flip" : "Rotate";

        public string NextAction => LastActionFlip ? "Rotate" : "Flip";


        public IRelayCommand? UpdateBedCommand { get; set; }
        public MainViewModel? ViewModel { get; }


        public Bed() { }

        public Bed(MainViewModel viewModel)
        {
            UpdateBedCommand = new RelayCommand(() => MainViewModel.UpdateBed(this));
            ViewModel = viewModel;
        }
    }
}
