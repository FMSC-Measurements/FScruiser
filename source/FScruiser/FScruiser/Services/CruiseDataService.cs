using Backpack;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public class CruiseDataService : ICruiseDataService
    {
        public CruiseDataService(DatastoreRedux dataStore)
        {
            DataStore = dataStore;
        }

        public DatastoreRedux DataStore { get; private set; }

        public Sale Sale
        {
            get
            {
                return DataStore.GetEntityCache(typeof(Sale)).Values.OfType<Sale>().FirstOrDefault();
            }
        }

        public IEnumerable<CuttingUnitModel> Units
        {
            get
            {
                return DataStore.GetEntityCache(typeof(CuttingUnitModel)).Values.OfType<CuttingUnitModel>();
            }
        }

        public IEnumerable<CuttingUnitModel> GetUnits()
        {
            return DataStore.From<CuttingUnitModel>()
                .Read();
        }
    }
}