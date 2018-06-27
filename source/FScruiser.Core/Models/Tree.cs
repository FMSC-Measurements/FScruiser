using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;
using System;

namespace FScruiser.Models
{
    [EntitySource("Tree")]
    public class Tree : INPC_Base
    {
        #region table fields

        [Field(Name = "Tree_GUID")]
        public string Tree_GUID { get; set; }

        [Field(Alias = "stratumCode", SQLExpression = "Stratum.Code")]
        public string StratumCode { get; set; }

        [Field(Alias = "sampleGroupCode", SQLExpression = "SampleGroup.Code")]
        public string SampleGroupCode { get; set; }

        [Field(Name = "TreeNumber")]
        public int TreeNumber { get; set; }

        [Field(Name = "Species")]
        public string Species { get; set; }

        [Field(Name = "CountOrMeasure")]
        public string CountOrMeasure { get; set; } = DEFAULT_COUNT_MEASURE;

        [Field(Name = "TreeCount")]
        public int TreeCount { get; set; }

        [Field(Name = "KPI")]
        public int KPI { get; set; }

        [Field(Name = "STM")]
        public string STM { get; set; } = DEFAULT_STM;

        [Field(Name = "SeenDefectPrimary")]
        public double SeenDefectPrimary { get; set; }

        [Field(Name = "SeenDefectSecondary")]
        public double SeenDefectSecondary { get; set; }

        [Field(Name = "RecoverablePrimary")]
        public double RecoverablePrimary { get; set; }

        [Field(Name = "HiddenPrimary")]
        public double HiddenPrimary { get; set; }

        [Field(Name = "Initials")]
        public string Initials { get; set; }

        [Field(Name = "LiveDead")]
        public string LiveDead { get; set; } = DEFAULT_LIVE_DEAD;

        [Field(Name = "Grade")]
        public string Grade { get; set; } = DEFAULT_GRADE;

        [Field(Name = "HeightToFirstLiveLimb")]
        public double HeightToFirstLiveLimb { get; set; }

        [Field(Name = "PoleLength")]
        public double PoleLength { get; set; }

        [Field(Name = "ClearFace")]
        public string ClearFace { get; set; } = DEFAULT_CLEAR_FACE;

        [Field(Name = "CrownRatio")]
        public double CrownRatio { get; set; }

        [Field(Name = "DBH")]
        public double DBH { get; set; }

        [Field(Name = "DRC")]
        public double DRC { get; set; }

        [Field(Name = "TotalHeight")]
        public virtual float TotalHeight { get; set; }

        [Field(Name = "MerchHeightPrimary")]
        public double MerchHeightPrimary { get; set; }

        [Field(Name = "MerchHeightSecondary")]
        public double MerchHeightSecondary { get; set; }

        [Field(Name = "FormClass")]
        public double FormClass { get; set; }

        [Field(Name = "UpperStemDiameter")]
        public double UpperStemDiameter { get; set; }

        [Field(Name = "UpperStemHeight")]
        public double UpperStemHeight { get; set; }

        [Field(Name = "DBHDoubleBarkThickness")]
        public double DBHDoubleBarkThickness { get; set; }

        [Field(Name = "TopDIBPrimary")]
        public double TopDIBPrimary { get; set; }

        [Field(Name = "TopDIBSecondary")]
        public double TopDIBSecondary { get; set; }

        [Field(Name = "DefectCode")]
        public string DefectCode { get; set; } = DEFAULT_DEFECT_CODE;

        [Field(Name = "DiameterAtDefect")]
        public double DiameterAtDefect { get; set; }

        [Field(Name = "VoidPercent")]
        public double VoidPercent { get; set; }

        [Field(Name = "Slope")]
        public double Slope { get; set; }

        [Field(Name = "Aspect")]
        public double Aspect { get; set; }

        [Field(Name = "Remarks")]
        public string Remarks { get; set; } = "";

        [Field(Name = "IsFallBuckScale")]
        public bool IsFallBuckScale { get; set; }

        [CreatedByField()]
        public string CreatedBy { get; set; }

        [InfrastructureFieldAttribute(Name = "CreatedDate",
        PersistMode = PersistMode.Never)]
        public DateTime CreatedDate { get; set; }

        [ModifiedByField()]
        public string ModifiedBy { get; set; }

        [InfrastructureFieldAttribute(Name = "ModifiedDate",
        PersistMode = PersistMode.Never)]
        public string ModifiedDate { get; set; }

        #endregion table fields

        private bool _hasFieldData;

        private static readonly string DEFAULT_STM = "N";
        private static readonly string DEFAULT_COUNT_MEASURE = "C";
        private static readonly string DEFAULT_LIVE_DEAD = "L";
        private static readonly string DEFAULT_GRADE = "00";
        private static readonly string DEFAULT_CLEAR_FACE = "";
        private static readonly string DEFAULT_DEFECT_CODE = "";

        //[IgnoreField]
        //protected bool HasError { get; set; }

        //[IgnoreField]
        //public bool HasFieldData
        //{
        //    get { return _hasFieldData; }
        //    set
        //    {
        //        _hasFieldData = value;
        //        NotifyPropertyChanged(nameof(HasFieldData));
        //        NotifyPropertyChanged(nameof(TallyFeedStatus));
        //    }
        //}

        //[IgnoreField]
        //public string TallyFeedStatus
        //{
        //    get
        //    {
        //        if (HasErrors())
        //        {
        //            return "Error";
        //        }
        //        else if (HasFieldData == false)
        //        {
        //            return "NoData";
        //        }
        //        else
        //        {
        //            return "HasData";
        //        }
        //    }
        //}
    }
}