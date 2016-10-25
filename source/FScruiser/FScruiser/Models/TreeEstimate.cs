using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("TreeEstimate")]
    public class TreeEstimate
    {
        public long? CountTree_CN { get; set; }

        public int KPI { get; set; }
    }
}