using CruiseDAL;
using System;

namespace FScruiser.Data
{
    public abstract class DataserviceBase
    {
        private CruiseDatastore_V3 _database;

        public CruiseDatastore_V3 Database
        {
            get { return _database; }
            set
            {
                _database = value;
                OnDatabaseChanged();
            }
        }

        private void OnDatabaseChanged()
        {
            var database = Database;
            if (database == null) { return; }

            //DatabaseUpdater.Update(database);
        }

        public DataserviceBase(string path)
        {
            var database = new CruiseDatastore_V3(path ?? throw new ArgumentNullException(nameof(path)));

            Database = database;
        }

        public DataserviceBase(CruiseDatastore_V3 database)
        {
            Database = database;
        }
    }
}