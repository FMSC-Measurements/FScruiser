using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;

namespace FScruiser.Models
{
    [EntitySource("Plot")]
    public class Plot : INPC_Base
    {
        private double _slope;
        private double _aspect;
        private string _remarks;

        [Field("PlotID")]
        public string PlotID { get; set; }

        [Field("CuttingUnitCode")]
        public string CuttingUnitCode { get; set; }

        [Field("PlotNumber")]
        public int PlotNumber { get; set; }

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

        [Field("Remarks")]
        public string Remarks
        {
            get => _remarks;
            set => SetValue(ref _remarks, value);
        }

        [Field("XCoordinate")]
        public double XCoordinate { get; set; }

        [Field("YCoordinate")]
        public double YCoordinate { get; set; }

        [Field("ZCoordinate")]
        public double ZCoordinate { get; set; }

        //public int CuttingUnit_CN { get; set; }
    }
}