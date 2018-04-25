using CruiseDAL.DataObjects;
using FMSC.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class SampleGroup : SampleGroupDO
    {
        public SampleSelecter Sampler { get; set; }
    }
}
