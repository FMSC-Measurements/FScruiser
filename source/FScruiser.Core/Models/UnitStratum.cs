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

        public Stratum Stratum { get; set; }

        public List<Plot> Plots { get; set; }

        public List<Tree> Trees { get; set; }
    }
}