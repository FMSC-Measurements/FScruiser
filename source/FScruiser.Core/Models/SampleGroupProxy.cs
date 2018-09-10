using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("SampleGroup")]
    public class SampleGroupProxy
    {
        [Field("SampleGroup_CN")]
        public int SampleGroup_CN { get; set; }

        [Field("Stratum_CN")]
        public int Stratum_CN { get; set; }

        [Field("Code")]
        public string Code { get; set; }

        [Field(Alias = "StratumCode", SQLExpression = "Stratum.Code")]
        public string StratumCode { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}
