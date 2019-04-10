using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class SamplerState
    {
        [Field("StratumCode")]
        public string StratumCode { get; set; }

        [Field("SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        [Field("Method")]
        public string Method { get; set; }

        [Field("SamplingFrequency")]
        public int SamplingFrequency { get; set; }

        [Field("InsuranceFrequency")]
        public int InsuranceFrequency { get; set; }

        [Field("KZ")]
        public int KZ { get; set; }

        [Field("SampleSelectorState")]
        public string SampleSelectorState { get; set; }

        [Field("SampleSelectorType")]
        public string SampleSelectorType { get; set; }
    }
}
