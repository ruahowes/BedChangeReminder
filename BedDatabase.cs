using System.Collections.ObjectModel;
using SQLite;

namespace BedChangeReminder
{
    public class BedDatabase
    {
        private readonly SQLiteConnection _database;

        public BedDatabase(string dbPath)
        {
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Bed>();
        }

        public ObservableCollection<Bed> GetBeds() => new ObservableCollection<Bed>(_database.Table<Bed>().ToList());
        public void AddBed(Bed bed) { _database.Insert(bed); }
        public void UpdateBed(Bed bed) { _database.Update(bed); }
    }
}
