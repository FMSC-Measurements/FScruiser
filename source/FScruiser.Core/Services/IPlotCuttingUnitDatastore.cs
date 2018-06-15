using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface IPlotCuttingUnitDatastore
    {
        int GetHighestTreeNumberInUnit(string unitCode, string stratumCode, int plotNumber);
    }
}
