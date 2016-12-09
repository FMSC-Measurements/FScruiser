using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    //[EntitySource(SourceName = "CuttingUnitStratum", JoinCommands = "JOIN Stratum USING (Stratum_CN)")]
    [Table("CuttingUnitStratum")]
    public class UnitStratum
    {
        [Key]
        public long RowID { get; set; }

        public long CuttingUnit_CN { get; set; }

        public long Stratum_CN { get; set; }

        [ForeignKey(nameof(Stratum_CN))]
        public Stratum Stratum { get; set; }

        [ForeignKey(nameof(CuttingUnit_CN))]
        public CuttingUnit Unit { get; set; }

        List<Plot> _plots;

        public List<Plot> Plots
        {
            get { return _plots ?? (_plots = new List<Plot>()); }
            set { _plots = value; }
        }

        [NotMapped]
        public List<Tree> Trees { get; set; }
    }
}