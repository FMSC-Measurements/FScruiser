using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.XF.Services
{
    public interface ICuttingUnitDatastoreProvider
    {
        ICuttingUnitDatastore CuttingUnitDatastore { get; set; }
        ISampleSelectorDataService SampleSelectorDataService { get; set; }
        string Cruise_Path { get; set; }
    }

    public class CuttingUnitDatastoreProvider : ICuttingUnitDatastoreProvider
    {
        public ICuttingUnitDatastore CuttingUnitDatastore { get; set; }
        public ISampleSelectorDataService SampleSelectorDataService { get; set; }
        public string Cruise_Path { get; set; }
    }
}
