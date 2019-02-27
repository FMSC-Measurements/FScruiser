using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class TallyPopulation_Base : INPC_Base
    {
        [Field("CountTree_CN")]
        public long? CountTree_CN { get; set; }

        [Field("Description")]
        public string TallyDescription { get; set; }

        [Field("HotKey")]
        public string TallyHotKey { get; set; }

        [Field("StratumCode")]
        public string StratumCode { get; set; }

        [Field("StratumMethod")]
        public string Method { get; set; }

        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(Method);

        [Field("SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        [Field("sgMinKPI")]
        public int MinKPI { get; set; }

        [Field("sgMaxKPI")]
        public int MaxKPI { get; set; }

        [Field("SampleGroup_CN")]
        public int SampleGroup_CN { get; set; }

        [Field("Species")]
        public string Species { get; set; }

        [Field("LiveDead")]
        public string LiveDead { get; set; }
    }
}
