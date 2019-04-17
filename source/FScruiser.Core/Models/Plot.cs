using FMSC.ORM.EntityModel.Attributes;

namespace FScruiser.Models
{
    [EntitySource("Plot")]
    public class Plot
    {
        [Field("PlotID")]
        public string PlotID { get; set; }

        [Field("CuttingUnitCode")]
        public string CuttingUnitCode { get; set; }

        [Field("PlotNumber")]
        public int PlotNumber { get; set; }

        [Field("Slope")]
        public double Slope { get; set; }

        [Field("Aspect")]
        public double Aspect { get; set; }

        [Field("Remarks")]
        public string Remarks { get; set; }

        [Field("XCoordinate")]
        public double XCoordinate { get; set; }

        [Field("YCoordinate")]
        public double YCoordinate { get; set; }

        [Field("ZCoordinate")]
        public double ZCoordinate { get; set; }

        //public int CuttingUnit_CN { get; set; }
    }
}