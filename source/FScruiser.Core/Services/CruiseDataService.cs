using CruiseDAL;
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
        private IEnumerable<CuttingUnit> _units;

        protected DAL Datastore {  get; set; }

        public string Path => Datastore.Path;
    
        public CruiseDataService(string path)
        {
            if(path == null) { throw new ArgumentNullException(nameof(path)); }

            Datastore = new DAL(path);
        }

        public IEnumerable<CuttingUnit> Units => _units ?? (_units = ReadUnits());

        private IEnumerable<CuttingUnit> ReadUnits()
        {
            return Datastore.From<CuttingUnit>()
                .Query().ToList();
        }
    }
}