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

        [Field(Alias = "unitCode", SQLExpression = "CuttingUnit.Code")]
        public string UnitCode
        {
            get { return _unitCode; }
            set { SetValue(ref _unitCode, value); }
        }

        [Field(Alias = "stratumCode", SQLExpression = "Stratum.Code")]
        public string StratumCode
        {
            get { return _stratumCode; }
            set { SetValue(ref _stratumCode, value); }
        }

        [Field(Alias = "sampleGroupCode", SQLExpression = "SampleGroup.Code")]
        public string SampleGroupCode
        {
            get { return _sampleGroupCode; }
            set { SetValue(ref _sampleGroupCode, value); }
        }

        [Field(Alias = "PlotNumber", SQLExpression = "Plot.PlotNumber")]
        public long? PlotNumber
        {
            get { return _plotNumber; }
            set { SetValue(ref _plotNumber, value); }
        }

        [Field(Name = "TreeNumber")]
        public int TreeNumber
        {
            get { return _treeNumber; }
            set { SetValue(ref _treeNumber, value); }
        }

        [Field(Name = "Species")]
        public string Species
        {
            get { return _species; }
            set { SetValue(ref _species, value); }
        }

        [Field(Name = "CountOrMeasure")]
        public string CountOrMeasure
        {
            get { return _countOrMeasure; }
            set { SetValue(ref _countOrMeasure, value); }
        }

        [Field(Name = "TreeCount")]
        public int TreeCount
        {
            get { return _treeCount; }
            set { SetValue(ref _treeCount, value); }
        }

        [Field(Name = "KPI")]
        public int KPI
        {
            get { return _kpi; }
            set { SetValue(ref _kpi, value); }
        }

        [Field(Name = "STM")]
        public string STM
        {
            get { return _stm; }
            set { SetValue(ref _stm, value); }
        }

        [Field(Name = "SeenDefectPrimary")]
        public double SeenDefectPrimary
        {
            get { return _seenDefectSecondary; }
            set { SetValue(ref _seenDefectSecondary, value); }
        }

        [Field(Name = "SeenDefectSecondary")]
        public double SeenDefectSecondary
        {
            get { return _seenDefectSecondary; }
            set { SetValue(ref _seenDefectSecondary, value); }
        }

        [Field(Name = "RecoverablePrimary")]
        public double RecoverablePrimary
        {
            get { return _recoverablePrimary; }
            set { SetValue(ref _recoverablePrimary, value); }
        }

        [Field(Name = "HiddenPrimary")]
        public double HiddenPrimary
        {
            get { return _hiddenPrimary; }
            set { SetValue(ref _hiddenPrimary, value); }
        }

        [Field(Name = "Initials")]
        public string Initials
        {
            get { return _initials; }
            set { SetValue(ref _initials, value); }
        }

        [Field(Name = "LiveDead")]
        public string LiveDead
        {
            get { return _liveDead; }
            set { SetValue(ref _liveDead, value); }
        }

        [Field(Name = "Grade")]
        public string Grade
        {
            get { return _grade; }
            set { SetValue(ref _grade, value); }
        }

        [Field(Name = "HeightToFirstLiveLimb")]
        public double HeightToFirstLiveLimb
        {
            get { return _heightToFirstLiveLimb; }
            set { SetValue(ref _heightToFirstLiveLimb, value); }
        }

        [Field(Name = "PoleLength")]
        public double PoleLength
        {
            get { return _poleLength; }
            set { SetValue(ref _poleLength, value); }
        }

        [Field(Name = "ClearFace")]
        public string ClearFace
        {
            get { return _clearFace; }
            set { SetValue(ref _clearFace, value); }
        }

        [Field(Name = "CrownRatio")]
        public double CrownRatio
        {
            get { return _crownRatio; }
            set { SetValue(ref _crownRatio, value); }
        }

        [Field(Name = "DBH")]
        public double DBH
        {
            get { return _dbh; }
            set { SetValue(ref _dbh, value); }
        }

        [Field(Name = "DRC")]
        public double DRC
        {
            get { return _drc; }
            set { SetValue(ref _drc, value); }
        }

        [Field(Name = "TotalHeight")]
        public virtual float TotalHeight
        {
            get { return _totalHeight; }
            set { SetValue(ref _totalHeight, value); }
        }

        [Field(Name = "MerchHeightPrimary")]
        public double MerchHeightPrimary
        {
            get { return _merchHeightPrimary; }
            set { SetValue(ref _merchHeightPrimary, value); }
        }

        [Field(Name = "MerchHeightSecondary")]
        public double MerchHeightSecondary
        {
            get { return _merchHeightSecondary; }
            set { SetValue(ref _merchHeightSecondary, value); }
        }

        [Field(Name = "FormClass")]
        public double FormClass
        {
            get { return _formClass; }
            set { SetValue(ref _formClass, value); }
        }

        [Field(Name = "UpperStemDiameter")]
        public double UpperStemDiameter
        {
            get { return _upperStemDiameter; }
            set { SetValue(ref _upperStemDiameter, value); }
        }

        [Field(Name = "UpperStemHeight")]
        public double UpperStemHeight
        {
            get { return _upperStemHeight; }
            set { SetValue(ref _upperStemHeight, value); }
        }

        [Field(Name = "DBHDoubleBarkThickness")]
        public double DBHDoubleBarkThickness
        {
            get { return _dbhDoubleBarkThickness; }
            set { SetValue(ref _dbhDoubleBarkThickness, value); }
        }

        [Field(Name = "TopDIBPrimary")]
        public double TopDIBPrimary
        {
            get { return _topDibPrimary; }
            set { SetValue(ref _topDibPrimary, value); }
        }

        [Field(Name = "TopDIBSecondary")]
        public double TopDIBSecondary
        {
            get { return _topDibSecondary; }
            set { SetValue(ref _topDibSecondary, value); }
        }

        [Field(Name = "DefectCode")]
        public string DefectCode
        {
            get { return _defectCode; }
            set { SetValue(ref _defectCode, value); }
        }

        [Field(Name = "DiameterAtDefect")]
        public double DiameterAtDefect
        {
            get { return _diameterAtDefect; }
            set { SetValue(ref _diameterAtDefect, value); }
        }

        [Field(Name = "VoidPercent")]
        public double VoidPercent
        {
            get { return _voidPercent; }
            set { SetValue(ref _voidPercent, value); }
        }

        [Field(Name = "Slope")]
        public double Slope
        {
            get { return _slope; }
            set { SetValue(ref _slope, value); }
        }

        [Field(Name = "Aspect")]
        public double Aspect
        {
            get { return _aspect; }
            set { SetValue(ref _aspect, value); }
        }

        [Field(Name = "Remarks")]
        public string Remarks
        {
            get { return _remarks; }
            set { SetValue(ref _remarks, value); }
        }

        [Field(Name = "IsFallBuckScale")]
        public bool IsFallBuckScale
        {
            get { return _isFallBuckScale; }
            set { SetValue(ref _isFallBuckScale, value); }
        }

        [CreatedByField()]
        public string CreatedBy
        {
            get { return _createdBy; }
            set { SetValue(ref _createdBy, value); }
        }

        [InfrastructureFieldAttribute(Name = "CreatedDate",
        PersistMode = PersistMode.Never)]
        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set { SetValue(ref _createdDate, value); }
        }

        [ModifiedByField()]
        public string ModifiedBy
        {
            get { return _modifiedBy; }
            set { SetValue(ref _modifiedBy, value); }
        }

        [InfrastructureFieldAttribute(Name = "ModifiedDate",
        PersistMode = PersistMode.Never)]
        public string ModifiedDate
        {
            get { return _modifiedDate; }
            set { SetValue(ref _modifiedDate, value); }
        }

        #endregion table fields

        private string _stratumCode;
        private string _sampleGroupCode;
        private int _treeNumber;
        private string _species;
        private string _countOrMeasure = DEFAULT_COUNT_MEASURE;
        private int _treeCount;
        private int _kpi;
        private string _stm = DEFAULT_STM;
        private double _seenDefectSecondary;
        private double _recoverablePrimary;
        private double _hiddenPrimary;
        private string _initials;
        private string _liveDead = DEFAULT_LIVE_DEAD;
        private string _grade = DEFAULT_GRADE;
        private double _heightToFirstLiveLimb;
        private double _poleLength;
        private string _clearFace = DEFAULT_CLEAR_FACE;
        private double _crownRatio;
        private double _dbh;
        private double _drc;
        private float _totalHeight;
        private double _merchHeightPrimary;
        private double _merchHeightSecondary;
        private double _formClass;
        private double _upperStemDiameter;
        private double _upperStemHeight;
        private double _dbhDoubleBarkThickness;
        private double _topDibPrimary;
        private double _topDibSecondary;
        private string _defectCode = DEFAULT_DEFECT_CODE;
        private double _diameterAtDefect;
        private double _voidPercent;
        private double _slope;
        private double _aspect;
        private string _remarks = "";
        private bool _isFallBuckScale;
        private string _createdBy;
        private DateTime _createdDate;
        private string _modifiedBy;
        private string _modifiedDate;
        private long? _plotNumber;
        private string _unitCode;
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