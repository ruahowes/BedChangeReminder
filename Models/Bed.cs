using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BedChangeReminder.Models
{
    public enum BedAction
    {
        None = 0,
        Flip = 1,
        Rotate = 2
    }

    public partial class Bed : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ObservableProperty]
        private string name = "NewBed";

        [ObservableProperty]
        private int frequency = 14; // Days

        [ObservableProperty]
        private DateTime lastChangeDate = DateTime.Today;

        [ObservableProperty]
        private BedAction lastAction = BedAction.Flip;

        public DateTime NextChangeDate => LastChangeDate.AddDays(Frequency);

        public string LastActionText => LastAction switch
        {
            BedAction.None => "None",
            BedAction.Flip => "Flip",
            BedAction.Rotate => "Rotate",
            _ => "Unknown"
        };

        public string NextActionText => LastAction switch
        {
            BedAction.None => "Flip or Rotate",
            BedAction.Flip => "Rotate",
            BedAction.Rotate => "Flip",
            _ => "Flip"
        };

        public int DaysUntilNext => (NextChangeDate - DateTime.Today).Days;

        public string DaysUntilNextText
        {
            get
            {
                var days = DaysUntilNext;
                return days switch
                {
                    < 0 => $"Overdue by {Math.Abs(days)} day(s)",
                    0 => "Due today!",
                    1 => "Due tomorrow",
                    _ => $"Due in {days} day(s)"
                };
            }
        }

        public bool IsOverdue => DaysUntilNext < 0;
        public bool IsDueToday => DaysUntilNext == 0;
        public bool IsDueSoon => DaysUntilNext <= 3 && DaysUntilNext > 0;

        // For backwards compatibility with existing database
        [Obsolete("Use LastAction instead")]
        public bool LastActionFlip
        {
            get => LastAction == BedAction.Flip;
            set => LastAction = value ? BedAction.Flip : BedAction.Rotate;
        }
    }
}