using FScruiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser
{
    public class ViewModelLocator
    {
        CuttingUnitListViewModel _cuttingUnitList;

        public CuttingUnitListViewModel CuttingUnitList
        {
            get
            {
                return _cuttingUnitList ?? (_cuttingUnitList = new CuttingUnitListViewModel());
            }
        }
    }
}