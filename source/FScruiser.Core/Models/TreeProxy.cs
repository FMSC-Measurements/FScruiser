using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [Table("Tree")]
    //[EntitySource(SourceName = "Tree", JoinCommands = "JOIN CuttingUnit ON (Tree.CuttingUnit_CN = CuttingUnit.CuttingUnit_CN) JOIN Stratum USING (Stratum_CN) LEFT JOIN SampleGroup USING (SampleGroup_CN) LEFT JOIN TreeDefaultValue USING (TreeDefaultValue_CN) LEFT JOIN Plot USING (Plot_CN)")]
    public class TreeProxy
    {
        [Key]
        public long Tree_CN { get; set; }

        public Guid Tree_GUID { get; set; }

        public int TreeNumber { get; set; }

        public string Species { get; set; }

        [ForeignKey(nameof(Stratum_CN))]
        public Stratum Stratum { get; set; }

        [ForeignKey(nameof(CuttingUnit_CN))]
        public CuttingUnit CuttingUnit { get; set; }

        [ForeignKey(nameof(SampleGroup_CN))]
        public SampleGroup SampleGroup { get; set; }

        [ForeignKey(nameof(Plot_CN))]
        public Plot Plot { get; set; }

        [ForeignKey(nameof(TreeDefaultValue_CN))]
        public TreeDefaultValue TDV { get; set; }

        public double Height => Math.Max(TotalHeight, MerchHeightPrimary);

        public double TotalHeight { get; set; }

        public double MerchHeightPrimary { get; set; }

        public double Diameter => Math.Max(DBH, DRC);

        public double DBH { get; set; }

        public double DRC { get; set; }

        public long? Stratum_CN { get; set; }

        public long? SampleGroup_CN { get; set; }

        public long? TreeDefaultValue_CN { get; set; }

        public long? CuttingUnit_CN { get; set; }

        public long? Plot_CN { get; set; }
    }
}