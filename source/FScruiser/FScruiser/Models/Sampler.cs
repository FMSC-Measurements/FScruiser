using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("SampleGroup")]
    public class Sampler
    {
        public long SampleGroup_CN { get; set; }

        public long SamplingFrequency { get; set; }

        public long InsuranceFrequency { get; set; }

        public long KZ { get; set; }

        public string SampleSelectorType { get; set; }

        public string SampleSelectorState { get; set; }
    }
}