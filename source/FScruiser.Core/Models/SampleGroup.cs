using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [Table("SampleGroup")]
    public class SampleGroup
    {
        [Key]
        public long SampleGroup_CN { get; set; }

        public long Stratum_CN { get; set; }

        [ForeignKey(nameof(Stratum_CN))]
        public Stratum Stratum { get; set; }

        public string Code { get; set; }

        public int SamplingFrequency { get; set; }

        public int InsuranceFrequency { get; set; }

        public int MinKPI { get; set; }

        public int MaxKPI { get; set; }

        public int KZ { get; set; }
        public string SampleSelectorType { get; set; }

        public string SampleSelectorState { get; set; }

        [NotMapped]
        public List<TreeDefaultValue> TreeDefaults { get; set; }
    }
}