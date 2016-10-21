using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("CuttingUnitStratum")]
    public class UnitStratum
    {
        public long? CuttingUnit_CN { get; set; }

        public long? Stratum_CN { get; set; }
    }
}