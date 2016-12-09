using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [Table("SampleGroupTreeDefaultValue")]
    public class SampleGroupTreeDefaultValue
    {
        [Key]
        public long RowId { get; set; }

        public long SampleGroup_CN { get; set; }

        public long TreeDefaultValue_CN { get; set; }

        [ForeignKey(nameof(SampleGroup_CN))]
        public SampleGroup SampleGroup { get; set; }

        [ForeignKey(nameof(TreeDefaultValue_CN))]
        public TreeDefaultValue TDV { get; set; }
    }
}