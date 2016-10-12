using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("Tree")]
    public class TreeEx
    {
        #region keyFields

        public Guid Tree_GUID { get; set; }

        [Field("Species")]
        public string SpeciesCode { get; set; }

        [Field("TreeNumber")]
        public int TreeNumber { get; set; }

        [Field("SampleGroup_CN")]
        public long? SampleGroup_CN { get; set; }

        [Field("Stratum_CN")]
        public long? Stratum_CN { get; set; }

        [Field("TreeDefaultValue_CN")]
        public long? TreeDefaultValue_CN { get; set; }

        [Field("CuttingUnit_CN")]
        public long? CuttingUnit_CN { get; set; }

        [Field("Plot_CN")]
        public long? Plot_CN { get; set; }

        #endregion keyFields

        public double Aspect { get; set; }

        public string ClearFace { get; set; }

        public double CrownRatio { get; set; }
        public double DBH { get; set; }

        public double DBHDoubleBarkThickness { get; set; }

        public double DefectCode { get; set; }

        public double DiameterAtDefect { get; set; }

        public double DRC { get; set; }

        public double FormClass { get; set; }

        public string Grade { get; set; }

        public double HeightToFirstLiveLimb { get; set; }

        public double Initials { get; set; }

        public bool IsFallBuckScale { get; set; }

        public string LiveDead { get; set; }

        public double MerchHeightPrimary { get; set; }

        public double MerchHeightSecondary { get; set; }
        public double PoleLength { get; set; }

        public double RecoverablePrimary { get; set; }

        public string Remarks { get; set; }
        public double SeenDefectPrimary { get; set; }

        public double SeenDefectSecondary { get; set; }

        public double Slope { get; set; }
        public double TopDIBPrimary { get; set; }

        public double TopDIBSecondary { get; set; }

        public double TotalHeight { get; set; }
        public double UpperStemDOB { get; set; }

        public double UpperStemHeight { get; set; }
        public double VoidPercent { get; set; }
    }
}