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

        [Field(Alias = "TallyDescription", SQLExpression = "Tally.Description", PersistanceFlags = PersistanceFlags.Never)]
        public string TallyDescription { get; set; }

        [Field(Alias = "TallyHotKey", SQLExpression = "Tally.HotKey", PersistanceFlags = PersistanceFlags.Never)]
        public string TallyHotKey { get; set; }

        [Field(Alias = "StratumCode", SQLExpression = "Stratum.Code", PersistanceFlags = PersistanceFlags.Never)]
        public string StratumCode { get; set; }

        [Field(Alias = "StratumMethod", SQLExpression = "Stratum.Method", PersistanceFlags = PersistanceFlags.Never)]
        public string Method { get; set; }

        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(Method);

        [Field(Alias = "SampleGroupCode", SQLExpression = "SampleGroup.Code", PersistanceFlags = PersistanceFlags.Never)]
        public string SampleGroupCode { get; set; }

        [Field(Alias = "sgMinKPI", SQLExpression = "SampleGroup.MinKPI", PersistanceFlags = PersistanceFlags.Never)]
        public int MinKPI { get; set; }

        [Field(Alias = "sgMaxKPI", SQLExpression = "SampleGroup.MaxKPI", PersistanceFlags = PersistanceFlags.Never)]
        public int MaxKPI { get; set; }

        [Field("SampleGroup_CN")]
        public int SampleGroup_CN { get; set; }

        [Field(Alias = "tdvSpecies", SQLExpression = "TreeDefaultValue.Species", PersistanceFlags = PersistanceFlags.Never)]
        public string Species { get; set; }

        [Field(Alias = "tdvLiveDead", SQLExpression = "TreeDefaultValue.LiveDead", PersistanceFlags = PersistanceFlags.Never)]
        public string LiveDead { get; set; }
    }
}
