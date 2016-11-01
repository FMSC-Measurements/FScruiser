using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource(SourceName = "Tree", JoinCommands = "JOIN CuttingUnit ON (Tree.CuttingUnit_CN = CuttingUnit.CuttingUnit_CN) JOIN Stratum USING (Stratum_CN) LEFT JOIN SampleGroup USING (SampleGroup_CN) LEFT JOIN TreeDefaultValue USING (TreeDefaultValue_CN) LEFT JOIN Plot USING (Plot_CN)")]
    public class TreeProxy
    {
        [PrimaryKeyField(Name = "Tree_GUID")]
        public Guid Tree_GUID { get; set; }

        public int TreeNumber { get; set; }

        [Field(SQLExpression = "TreeDefaultValue.Species", Alias = "SpeciesCode")]
        public string SpeciesCode { get; set; }

        [Field(SQLExpression = "Stratum.Code", Alias = "StratumCode")]
        public string StratumCode { get; set; }

        [Field(SQLExpression = "CuttingUnit.Code", Alias = "CuttingUnitCode")]
        public string CuttingUnitCode { get; set; }

        [Field(SQLExpression = "SampleGroup.Code", Alias = "SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        [Field(SQLExpression = "ifnull(Plot.PlotNumber, 0)", Alias = "PlotNumber")]
        public int PlotNumber { get; set; }

        [Field(SQLExpression = "max(TotalHeight, MerchHeightPrimary)", Alias = "Height")]
        public double Height { get; set; }

        [Field(SQLExpression = "max(DBH, DRC)", Alias = "Diameter")]
        public double Diameter { get; set; }

        public long? Stratum_CN { get; set; }

        public long? SampleGroup_CN { get; set; }

        public long? TreeDefaultValue_CN { get; set; }

        public long? CuttingUnit_CN { get; set; }

        public long? Plot_CN { get; set; }

        //public string CountOrMeasure { get; set; }

        //public int TreeCount { get; set; }

        //public int KPI { get; set; }

        //public bool STM { get; set; }

        //public TreeSpecies Species
        //{
        //    get { return SpeciesOptions.First((x) => x.TreeDefaultValue_CN == this.TreeDefaultValue_CN); }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            SpeciesCode = value.SpeciesCode;
        //            TreeDefaultValue_CN = value.TreeDefaultValue_CN;
        //        }
        //    }
        //}

        //public TreeSampleGroup SampleGroup
        //{
        //    get { return SampleGroupOptions.FirstOrDefault((x) => x.SampleGroup_CN == this.SampleGroup_CN); }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            SampleGroup_CN = value.SampleGroup_CN;
        //        }
        //    }
        //}

        //public int SampleGroupIndex
        //{
        //    get { return SampleGroupOptions.IndexOf(SampleGroup); }
        //    set { SampleGroup = SampleGroupOptions.ElementAtOrDefault(value); }
        //}

        //public IList<TreeSampleGroup> SampleGroupOptions { get; set; }

        //public int SpeciesIndex
        //{
        //    get { return SpeciesOptions.IndexOf(Species); }
        //    set { Species = SpeciesOptions.ElementAtOrDefault(value); }
        //}
        //public IList<TreeSpecies> SpeciesOptions { get; set; }
        //public StratumModel Stratum { get; set; }

        //public double SeenDefectPrimary { get; set; }

        //public double SeenDefectSecondary { get; set; }

        //public double RecoverablePrimary { get; set; }

        //public double Initials { get; set; }

        //public string LiveDead { get; set; }

        //public string Grade { get; set; }

        //public double HeightToFirstLiveLimb { get; set; }

        //public double PoleLength { get; set; }

        //public string ClearFace { get; set; }

        //public double CrownRatio { get; set; }

        //public double DBH { get; set; }

        //public double DRC { get; set; }

        //public double TotalHeight { get; set; }

        //public double MerchHeightPrimary { get; set; }

        //public double MerchHeightSecondary { get; set; }

        //public double FormClass { get; set; }

        //public double UpperStemDOB { get; set; }

        //public double UpperStemHeight { get; set; }

        //public double DBHDoubleBarkThickness { get; set; }

        //public double TopDIBPrimary { get; set; }

        //public double TopDIBSecondary { get; set; }

        //public double DefectCode { get; set; }

        //public double DiameterAtDefect { get; set; }

        //public double VoidPercent { get; set; }

        //public double Slope { get; set; }

        //public double Aspect { get; set; }

        //public string Remarks { get; set; }

        //public bool IsFallBuckScale { get; set; }
    }
}