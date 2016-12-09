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

        public long Stratum_CN { get; set; }

        public long CuttingUnit_CN { get; set; }

        public int PlotNumber { get; set; }

        //bool _isEmpty = false;

        public bool? IsEmpty { get; set; } = false;
        //{
        //    get { return _isEmpty; }
        //    set { _isEmpty = value ?? false; }
        //}

        public double Slope { get; set; }

        public string Remarks { get; set; }

        [Required]
        public string CreatedBy { get; set; } = "Default";

        public List<Tree> Trees { get; set; } = new List<Tree>();

        [ForeignKey(nameof(Stratum_CN))]
        public Stratum Stratum { get; set; }

        public UnitStratum UnitStratum { get; set; }

        public override string ToString()
        {
            var isEmptyExpr = (IsEmpty.GetValueOrDefault(false)) ? ":Empty" : string.Empty;
            return $"{PlotNumber} {isEmptyExpr}";
        }
    }
}