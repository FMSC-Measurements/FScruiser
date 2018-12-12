using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public enum FixCNTTallyField { Unknown, DBH, TotalHeight, DRC };

    public class FixCntTallyPopulation
    {
        [Field("StratumCode")]
        public string StratumCode { get; set; }

        [Field("SGCode")]
        public string SGCode { get; set; }

        [Field("tdvSpecies")]
        public string Species { get; set; }

        //[Field("tdvLiveDead")]
        //public string LiveDead { get; set; }

        [Field("TreeDefaultValue_CN")]
        public int TreeDefaultValue_CN { get; set; }

        [Field("FieldName")]
        public FixCNTTallyField FieldName { get; set; }

        [Field("IntervalMin")]
        public int IntervalMin { get; set; }

        [Field("IntervalMax")]
        public int IntervalMax { get; set; }

        [Field("IntervalSize")]
        public int IntervalSize { get; set; }

        public List<FixCNTTallyBucket> Buckets { get; set; }
    }
}
