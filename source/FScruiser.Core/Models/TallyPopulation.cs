using CruiseDAL.DataObjects;
using FMSC.ORM.EntityModel.Attributes;
using System.Linq;

namespace FScruiser.Models
{
    public class TallyPopulation : CountTreeDO
    {
        [Field(Alias = "TallyDescription", SQLExpression = "Tally.Description")]
        public string TallyDescription { get; set; }

        [Field(Alias = "TallyHotKey", SQLExpression = "Tally.HotKey")]
        public string TallyHotKey { get; set; }

        [Field(Alias = "StratumCode", SQLExpression = "Stratum.Code")]
        public string StratumCode { get; set; }

        [Field(Alias = "StratumMethod", SQLExpression = "Stratum.Method")]
        public string Method { get; set; }

        public bool Is3P => CruiseMethods.THREE_P_METHODS.Contains(Method);

        [Field(Alias = "SampleGroupCode", SQLExpression = "SampleGroup.Code")]
        public string SampleGroupCode { get; set; }

        public SampleGroup SampleGroup { get; set; }
    }
}