﻿using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;

namespace FScruiser.Models
{
    [EntitySource("Plot_Stratum")]
    public class Plot_Stratum : INPC_Base
    {
        private int _plotNumber;
        private bool _inCruise;
        private bool _isEmpty;
        private int _kpi;
        private string _remarks;
        private double _slope;
        private double _aspect;

        [Field("Plot_Stratum_CN")]
        public long? Plot_Stratum_CN { get; set; }

        [Field("InCruise")]
        public bool InCruise
        {
            get => _inCruise;
            set => SetValue(ref _inCruise, value);
        }

        //[Field("PlotID")]
        //public string PlotID { get; set; }

        [Field("PlotNumber")]
        public int PlotNumber
        {
            get => _plotNumber;
            set => SetValue(ref _plotNumber, value);
        }

        [Field(Alias = "CuttingUnitCode")]
        public string CuttingUnitCode { get; set; }

        [Field(Alias = "StratumCode")]
        public string StratumCode { get; set; }

        [Field(Alias = "CruiseMethod", PersistanceFlags = PersistanceFlags.Never)]
        public string CruiseMethod { get; set; }

        [Field(Alias = "BAF", PersistanceFlags = PersistanceFlags.Never)]
        public double BAF { get; set; }

        [Field(Alias = "FPS", PersistanceFlags = PersistanceFlags.Never)]
        public double FPS { get; set; }

        
        [Field("IsEmpty")]
        public bool IsEmpty
        {
            get => _isEmpty;
            set => SetValue(ref _isEmpty, value);
        }

        [Field("KPI")]
        public int KPI
        {
            get => _kpi;
            set => SetValue(ref _kpi, value);
        }

        [Field(Alias = "KZ3PPNT", PersistanceFlags = PersistanceFlags.Never)]
        public int KZ3PPNT { get; set; }

        public int ThreePRandomValue { get; set; }


    }
}