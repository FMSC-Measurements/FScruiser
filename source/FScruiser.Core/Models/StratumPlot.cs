using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;
using System;

namespace FScruiser.Models
{
    [EntitySource("Plot")]
    public class StratumPlot : INPC_Base
    {
        private string _plot_guid;
        private string _stratumCode;
        private int _plotNumber;
        private bool _inCruise;
        private string _isEmpty;
        private int _kpi;
        private string _remarks;

        [Field("Plot_GUID")]
        public string Plot_GUID
        {
            get { return _plot_guid; }
            set { SetValue(ref _plot_guid, value); }
        }

        [Field(Alias = "StratumCode", SQLExpression = "Stratum.Code", PersistanceFlags = PersistanceFlags.Never)]
        public string StratumCode
        {
            get { return _stratumCode; }
            set { SetValue(ref _stratumCode, value); }
        }

        [Field("PlotNumber")]
        public int PlotNumber
        {
            get { return _plotNumber; }
            set { SetValue(ref _plotNumber, value); }
        }

        [Field(Alias = "InCruise")]
        public bool InCruise
        {
            get { return _inCruise; }
            set
            {
                SetValue(ref _inCruise, value);
            }
        }

        [Field("IsEmpty")]
        public string IsEmpty
        {
            get { return _isEmpty; }
            set { SetValue(ref _isEmpty, value); }
        }

        [Field("KPI")]
        public int KPI
        {
            get { return _kpi; }
            set { SetValue(ref _kpi, value); }
        }

        [Field("Remarks")]
        public string Remarks
        {
            get { return _remarks; }
            set { SetValue(ref _remarks, value); }
        }
    }
}