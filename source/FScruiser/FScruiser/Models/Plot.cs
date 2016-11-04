using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [Table("Plot")]
    public class Plot
    {
        [Key]
        public long Plot_CN { get; set; }

        [ForeignKey("Stratum")]
        public long Stratum_CN { get; set; }

        [ForeignKey("CuttingUnit")]
        public long CuttingUnit_CN { get; set; }

        public int PlotNumber { get; set; }

        public bool IsEmpty { get; set; }

        public string Remarks { get; set; }

        public Stratum Stratum { get; set; }

        public override string ToString()
        {
            var isEmptyExpr = (IsEmpty) ? ":Empty" : string.Empty;
            return $"{PlotNumber} {isEmptyExpr}";
        }
    }
}