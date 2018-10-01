using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;

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
        public string Plot_GUID { get; set; }

        [Field(Alias = "StratumCode", SQLExpression = "Stratum.Code", PersistanceFlags = PersistanceFlags.Never)]
        public string StratumCode { get; set; }

        [Field(Alias = "CruiseMethod", SQLExpression = "Stratum.Method", PersistanceFlags = PersistanceFlags.Never)]
        public string CruiseMethod { get; set; }

        [Field(Alias = "BAF", SQLExpression = "Stratum.BasalAreaFactor", PersistanceFlags = PersistanceFlags.Never)]
        public double BAF { get; set; }

        [Field(Alias = "FPS", SQLExpression = "Stratum.FixedPlotSize", PersistanceFlags = PersistanceFlags.Never)]
        public double FPS { get; set; }

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

        [Field("Slope")]
        public double Slope
        { get; set; }

        [Field("Aspect")]
        public double Aspect
        { get; set; }
    }
}