using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("Plot")]
    public class Plot
    {
        public int PlotNumber { get; set; }

        public int CuttingUnit_CN { get; set; }

        public IList<StratumPlot> Plots { get; protected set; } = new List<StratumPlot>();
    }
}