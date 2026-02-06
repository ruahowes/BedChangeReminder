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
        [NotifyPropertyChangedFor(nameof(NextChangeDate))]
        [NotifyPropertyChangedFor(nameof(DaysUntilNext))]
        [NotifyPropertyChangedFor(nameof(DaysUntilNextText))]
        [NotifyPropertyChangedFor(nameof(IsOverdue))]
        [NotifyPropertyChangedFor(nameof(IsDueToday))]
        [NotifyPropertyChangedFor(nameof(IsDueSoon))]
        private int frequency = 14; // Days

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NextChangeDate))]
        [NotifyPropertyChangedFor(nameof(DaysUntilNext))]
        [NotifyPropertyChangedFor(nameof(DaysUntilNextText))]
        [NotifyPropertyChangedFor(nameof(IsOverdue))]
        [NotifyPropertyChangedFor(nameof(IsDueToday))]
        [NotifyPropertyChangedFor(nameof(IsDueSoon))]
        private DateTime lastChangeDate = DateTime.Today;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LastActionText))]
        [NotifyPropertyChangedFor(nameof(NextActionText))]
        private BedAction lastAction = BedAction.Flip;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WeeksSinceMattressProtectorChange))]
        [NotifyPropertyChangedFor(nameof(MattressProtectorStatusText))]
        [NotifyPropertyChangedFor(nameof(MattressProtectorSummaryText))]
        private DateTime? lastMattressProtectorChangeDate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WeeksSincePillowLinerChange))]
        [NotifyPropertyChangedFor(nameof(PillowLinerStatusText))]
        [NotifyPropertyChangedFor(nameof(PillowLinerSummaryText))]
        private DateTime? lastPillowLinerChangeDate;

        // --- Computed properties (read-only, not persisted by SQLite) ---

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

        public int WeeksSinceMattressProtectorChange =>
            LastMattressProtectorChangeDate == null
                ? -1
                : (DateTime.Today - LastMattressProtectorChangeDate.Value).Days / 7;

        public int WeeksSincePillowLinerChange =>
            LastPillowLinerChangeDate == null
                ? -1
                : (DateTime.Today - LastPillowLinerChangeDate.Value).Days / 7;

        public string MattressProtectorStatusText
        {
            get
            {
                if (LastMattressProtectorChangeDate == null)
                    return "Mattress protector: Never changed";

                var weeks = WeeksSinceMattressProtectorChange;
                return $"Mattress protector: {LastMattressProtectorChangeDate.Value:dd MMM yyyy} ({weeks} week{(weeks != 1 ? "s" : "")} ago)";
            }
        }

        public string PillowLinerStatusText
        {
            get
            {
                if (LastPillowLinerChangeDate == null)
                    return "Pillow liners: Never changed";

                var weeks = WeeksSincePillowLinerChange;
                return $"Pillow liners: {LastPillowLinerChangeDate.Value:dd MMM yyyy} ({weeks} week{(weeks != 1 ? "s" : "")} ago)";
            }
        }

        // Short summary (for main page cards)
        public string MattressProtectorSummaryText =>
            FormatWeeksSummary(WeeksSinceMattressProtectorChange);

        public string PillowLinerSummaryText =>
            FormatWeeksSummary(WeeksSincePillowLinerChange);

        private static string FormatWeeksSummary(int weeks) => weeks switch
        {
            -1 => "Never",
            _ => $"{weeks} weeks ago"
        };

        // For backwards compatibility with existing database
        [Obsolete("Use LastAction instead")]
        public bool LastActionFlip
        {
            get => LastAction == BedAction.Flip;
            set => LastAction = value ? BedAction.Flip : BedAction.Rotate;
        }
    }
}