using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("CountTree")]
    public class CountTree
    {
        [Field("CuttingUnit_CN")]
        public long? CuttingUnit_CN { get; set; }

        [Field("SampleGroup_CN")]
        public long? SampleGroup_CN { get; set; }

        [Field("TreeCount")]
        public int TreeCount { get; set; }

        [Field("Description")]
        public string Description { get; set; }
    }
}