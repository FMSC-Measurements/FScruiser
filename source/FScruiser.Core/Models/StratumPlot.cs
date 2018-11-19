using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;

namespace FScruiser.Models
{
    [EntitySource("Plot")]
    public class StratumPlot : INPC_Base
    {
        private int _plotNumber;
        private bool _inCruise;
        private string _isEmpty;
        private int _kpi;
        private string _remarks;
        private double _slope;
        private double _aspect;

        [IgnoreField]
        public bool InCruise
        {
            get => _inCruise;
            set => SetValue(ref _inCruise, value);
        }

        [IgnoreField]
        public bool IsEmptyBool
        {
            get => this.IsEmpty == "True";
            set => this.IsEmpty = (value) ? "True" : "False";
        }

        [Field("Plot_GUID")]
        public string Plot_GUID { get; set; }

        [Field(Alias = "UnitCode", PersistanceFlags = PersistanceFlags.Never)]
        public string UnitCode { get; set; }

        [Field(Alias = "StratumCode", PersistanceFlags = PersistanceFlags.Never)]
        public string StratumCode { get; set; }

        [Field(Alias = "CruiseMethod", PersistanceFlags = PersistanceFlags.Never)]
        public string CruiseMethod { get; set; }

        [Field(Alias = "BAF", PersistanceFlags = PersistanceFlags.Never)]
        public double BAF { get; set; }

        [Field(Alias = "FPS", PersistanceFlags = PersistanceFlags.Never)]
        public double FPS { get; set; }

        [Field("PlotNumber")]
        public int PlotNumber
        {
            get => _plotNumber;
            set => SetValue(ref _plotNumber, value);
        }

        [Field("IsEmpty")]
        public string IsEmpty
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

        [Field(Alias = "KZ", PersistanceFlags = PersistanceFlags.Never)]
        public int KZ { get; set; }

        [Field("Remarks")]
        public string Remarks
        {
            get => _remarks;
            set => SetValue(ref _remarks, value);
        }

        [Field("Slope")]
        public double Slope
        {
            get => _slope;
            set => SetValue(ref _slope, value);
        }

        [Field("Aspect")]
        public double Aspect
        {
            get => _aspect;
            set => SetValue(ref _aspect, value);
        }
    }
}