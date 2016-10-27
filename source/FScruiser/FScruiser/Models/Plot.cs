using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class Plot
    {
        public int PlotNumber { get; set; }

        public long Stratum_CN { get; set; }

        public long CuttingUnit_CN { get; set; }
    }
}