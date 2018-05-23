using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;
using System.Linq;

namespace FScruiser.Models
{
    [EntitySource("CountTree")]
    public class TallyPopulation : INPC_Base
    {
        int _treeCount;
        int _sumKPI;

        [Field("CountTree_CN")]
        public int CountTree_CN { get; set; }

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

        [Field("SampleGroup_CN")]
        public int SampleGroup_CN { get; set; }

        [Field(Alias = "tdvSpecies", SQLExpression = "TreeDefaultValue.Species", PersistanceFlags = PersistanceFlags.Never)]
        public string Species { get; set; }

        public SampleGroup SampleGroup { get; set; }

        public CountTree Count { get; set; }
    }
}