using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [Table("TreeEstimate")]
    public class TreeEstimate
    {
        [Key]
        public long RowID { get; set; }

        public long? CountTree_CN { get; set; }

        public int KPI { get; set; }
    }
}