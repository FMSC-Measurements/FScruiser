using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [Table("TreeFieldSetup")]
    public class TreeField
    {
        public long Stratum_CN { get; set; }

        public string Field { get; set; }

        public string Heading { get; set; }

        public int FieldOrder { get; set; }

        [ForeignKey("Stratum_CN")]
        public Stratum Stratum { get; set; }
    }
}