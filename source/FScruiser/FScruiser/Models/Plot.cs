using Backpack.EntityModel.Attributes;
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
        [PrimaryKeyField(Name = "Plot_CN")]
        public long Plot_CN { get; set; }

        public int PlotNumber { get; set; }

        public long Stratum_CN { get; set; }

        public long CuttingUnit_CN { get; set; }
    }
}