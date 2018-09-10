using FMSC.ORM.EntityModel.Attributes;

namespace FScruiser.Models
{
    [EntitySource("Plot")]
    public class Plot
    {
        [Field("PlotNumber")]
        public int PlotNumber { get; set; }

        [Field("Slope")]
        public double Slope { get; set; }

        [Field("Aspect")]
        public double Aspect { get; set; }

        [Field("XCoordinate")]
        public double XCoordinate { get; set; }

        [Field("YCoordinate")]
        public double YCoordinate { get; set; }

        //public int CuttingUnit_CN { get; set; }
    }
}