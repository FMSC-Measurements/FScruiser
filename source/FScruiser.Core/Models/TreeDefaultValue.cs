using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FScruiser.Models
{
    [Table("TreeDefaultValue")]
    public class TreeDefaultValue
    {
        [Key]
        public long TreeDefaultValue_CN { get; set; }

        public string Species { get; set; }
    }
}