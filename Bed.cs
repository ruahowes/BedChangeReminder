using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BedChangeReminder
{
    public partial class Bed : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Frequency { get; set; } // Days
        public DateTime LastChangeDate { get; set; }
        public bool LastActionFlip { get; set; } // True = Flip, False = Rotate

        public DateTime NextChangeDate => LastChangeDate.AddDays(Frequency);
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
