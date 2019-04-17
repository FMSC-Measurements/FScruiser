using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;
using System;

namespace FScruiser.Models
{
    [EntitySource("Tree_V3")]
    public class Tree : INPC_Base
    {
        #region table fields

        [Field(Name = "TreeID")]
        public string TreeID { get; set; }

        [Field("CuttingUnitCode")]
        public string CuttingUnitCode
        {
            get { return _unitCode; }
            set { SetValue(ref _unitCode, value); }
        }

        [Field("StratumCode")]
        public string StratumCode
        {
            get { return _stratumCode; }
            set { SetValue(ref _stratumCode, value); }
        }

        [Field("SampleGroupCode")]
        public string SampleGroupCode
        {
            get { return _sampleGroupCode; }
            set { SetValue(ref _sampleGroupCode, value); }
        }

        [Field("PlotNumber")]
        public long? PlotNumber
        {
            get { return _plotNumber; }
            set { SetValue(ref _plotNumber, value); }
        }

        [Field("TreeNumber")]
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

        

        [Field(Name = "LiveDead")]
        public string LiveDead
        {
            get { return _liveDead; }
            set { SetValue(ref _liveDead, value); }
        }



        #endregion table fields

        private string _stratumCode;
        private string _sampleGroupCode;
        private int _treeNumber;
        private string _species;
        private string _countOrMeasure = DEFAULT_COUNT_MEASURE;

        
        private string _liveDead = DEFAULT_LIVE_DEAD;

        private string _remarks = "";

        private string _createdBy;
        private DateTime _createdDate;
        private string _modifiedBy;
        private string _modifiedDate;
        private long? _plotNumber;
        private string _unitCode;
        private static readonly string DEFAULT_STM = "N";
        private static readonly string DEFAULT_COUNT_MEASURE = "C";
        private static readonly string DEFAULT_LIVE_DEAD = "L";

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