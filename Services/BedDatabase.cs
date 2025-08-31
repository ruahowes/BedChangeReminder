using BedChangeReminder.Models;
using SQLite;

namespace BedChangeReminder.Services
{
    public class BedDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public BedDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "BedDatabase.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Bed>().Wait();
        }

        public Task<List<Bed>> GetBedsAsync()
        {
            return _database.Table<Bed>().ToListAsync();
        }

        public Task<int> SaveBedAsync(Bed bed)
        {
            if (bed.Id == 0)  // If Id is 0, it's a new entry
                return _database.InsertAsync(bed);
            else
                return _database.UpdateAsync(bed);
        }

        public Task<int> DeleteBedAsync(Bed bed)
        {
            return _database.DeleteAsync(bed);
        }
    }
}
