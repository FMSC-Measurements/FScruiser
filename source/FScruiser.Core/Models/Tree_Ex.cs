using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class Tree_Ex :Tree
    {
        private static readonly string DEFAULT_GRADE = "00";
        private static readonly string DEFAULT_CLEAR_FACE = "";
        private static readonly string DEFAULT_DEFECT_CODE = "";

        private double _seenDefectSecondary;
        private double _recoverablePrimary;
        private double _hiddenPrimary;
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
        private bool _isFallBuckScale;
        private string _remarks = "";
        private string _initials;

        //[Field(Name = "TreeID")]
        //public string TreeID { get; set; }

        [Field(Name = "IsFallBuckScale", SourceName = "TreeMeasurment")]
        public bool IsFallBuckScale
        {
            get { return _isFallBuckScale; }
            set { SetValue(ref _isFallBuckScale, value); }
        }

        [Field(Name = "SeenDefectPrimary", SourceName = "TreeMeasurment")]
        public double SeenDefectPrimary
        {
            get { return _seenDefectSecondary; }
            set { SetValue(ref _seenDefectSecondary, value); }
        }

        [Field(Name = "SeenDefectSecondary", SourceName = "TreeMeasurment")]
        public double SeenDefectSecondary
        {
            get { return _seenDefectSecondary; }
            set { SetValue(ref _seenDefectSecondary, value); }
        }

        [Field(Name = "RecoverablePrimary", SourceName = "TreeMeasurment")]
        public double RecoverablePrimary
        {
            get { return _recoverablePrimary; }
            set { SetValue(ref _recoverablePrimary, value); }
        }

        [Field(Name = "HiddenPrimary", SourceName = "TreeMeasurment")]
        public double HiddenPrimary
        {
            get { return _hiddenPrimary; }
            set { SetValue(ref _hiddenPrimary, value); }
        }

        [Field(Name = "Grade", SourceName = "TreeMeasurment")]
        public string Grade
        {
            get { return _grade; }
            set { SetValue(ref _grade, value); }
        }

        [Field(Name = "HeightToFirstLiveLimb", SourceName = "TreeMeasurment")]
        public double HeightToFirstLiveLimb
        {
            get { return _heightToFirstLiveLimb; }
            set { SetValue(ref _heightToFirstLiveLimb, value); }
        }

        [Field(Name = "PoleLength", SourceName = "TreeMeasurment")]
        public double PoleLength
        {
            get { return _poleLength; }
            set { SetValue(ref _poleLength, value); }
        }

        [Field(Name = "ClearFace", SourceName = "TreeMeasurment")]
        public string ClearFace
        {
            get { return _clearFace; }
            set { SetValue(ref _clearFace, value); }
        }

        [Field(Name = "CrownRatio", SourceName = "TreeMeasurment")]
        public double CrownRatio
        {
            get { return _crownRatio; }
            set { SetValue(ref _crownRatio, value); }
        }

        [Field(Name = "DBH", SourceName = "TreeMeasurment")]
        public double DBH
        {
            get { return _dbh; }
            set { SetValue(ref _dbh, value); }
        }

        [Field(Name = "DRC", SourceName = "TreeMeasurment")]
        public double DRC
        {
            get { return _drc; }
            set { SetValue(ref _drc, value); }
        }

        [Field(Name = "TotalHeight", SourceName = "TreeMeasurment")]
        public virtual float TotalHeight
        {
            get { return _totalHeight; }
            set { SetValue(ref _totalHeight, value); }
        }

        [Field(Name = "MerchHeightPrimary", SourceName = "TreeMeasurment")]
        public double MerchHeightPrimary
        {
            get { return _merchHeightPrimary; }
            set { SetValue(ref _merchHeightPrimary, value); }
        }

        [Field(Name = "MerchHeightSecondary", SourceName = "TreeMeasurment")]
        public double MerchHeightSecondary
        {
            get { return _merchHeightSecondary; }
            set { SetValue(ref _merchHeightSecondary, value); }
        }

        [Field(Name = "FormClass", SourceName = "TreeMeasurment")]
        public double FormClass
        {
            get { return _formClass; }
            set { SetValue(ref _formClass, value); }
        }

        [Field(Name = "UpperStemDiameter", SourceName = "TreeMeasurment")]
        public double UpperStemDiameter
        {
            get { return _upperStemDiameter; }
            set { SetValue(ref _upperStemDiameter, value); }
        }

        [Field(Name = "UpperStemHeight", SourceName = "TreeMeasurment")]
        public double UpperStemHeight
        {
            get { return _upperStemHeight; }
            set { SetValue(ref _upperStemHeight, value); }
        }

        [Field(Name = "DBHDoubleBarkThickness", SourceName = "TreeMeasurment")]
        public double DBHDoubleBarkThickness
        {
            get { return _dbhDoubleBarkThickness; }
            set { SetValue(ref _dbhDoubleBarkThickness, value); }
        }

        [Field(Name = "TopDIBPrimary", SourceName = "TreeMeasurment")]
        public double TopDIBPrimary
        {
            get { return _topDibPrimary; }
            set { SetValue(ref _topDibPrimary, value); }
        }

        [Field(Name = "TopDIBSecondary", SourceName = "TreeMeasurment")]
        public double TopDIBSecondary
        {
            get { return _topDibSecondary; }
            set { SetValue(ref _topDibSecondary, value); }
        }

        [Field(Name = "DefectCode", SourceName = "TreeMeasurment")]
        public string DefectCode
        {
            get { return _defectCode; }
            set { SetValue(ref _defectCode, value); }
        }

        [Field(Name = "DiameterAtDefect", SourceName = "TreeMeasurment")]
        public double DiameterAtDefect
        {
            get { return _diameterAtDefect; }
            set { SetValue(ref _diameterAtDefect, value); }
        }

        [Field(Name = "VoidPercent", SourceName = "TreeMeasurment")]
        public double VoidPercent
        {
            get { return _voidPercent; }
            set { SetValue(ref _voidPercent, value); }
        }

        [Field(Name = "Slope", SourceName = "TreeMeasurment")]
        public double Slope
        {
            get { return _slope; }
            set { SetValue(ref _slope, value); }
        }

        [Field(Name = "Aspect", SourceName = "TreeMeasurment")]
        public double Aspect
        {
            get { return _aspect; }
            set { SetValue(ref _aspect, value); }
        }

        [Field(Name = "Remarks", SourceName = "TreeMeasurment")]
        public string Remarks
        {
            get { return _remarks; }
            set { SetValue(ref _remarks, value); }
        }

        [Field(Name = "Initials", SourceName = "TreeMeasurment")]
        public string Initials
        {
            get { return _initials; }
            set { SetValue(ref _initials, value); }
        }
    }
}
