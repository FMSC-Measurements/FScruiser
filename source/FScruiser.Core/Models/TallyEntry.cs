using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Models;

namespace FScruiser.Models
{
    [EntitySource("TallyLedger")]
    public class TallyEntry : IHasTreeID
    {
        public TallyEntry()
        { }

        public TallyEntry(TallyAction action)
        {
            CuttingUnitCode = action.CuttingUnitCode;
            PlotNumber = action.PlotNumber;
            StratumCode = action.StratumCode;
            SampleGroupCode = action.SampleGroupCode;
            Species = action.Species;
            LiveDead = action.LiveDead;
            EntryType = action.EntryType;
        }

        [Field("CuttingUnitCode")]
        public string CuttingUnitCode { get; set; }

        [Field("PlotNumber")]
        public int? PlotNumber { get; set; }

        [Field("StratumCode")]
        public string StratumCode { get; set; }

        [Field("SampleGroupCode")]
        public string SampleGroupCode { get; set; }

        [Field("ErrorCount")]
        public int ErrorCount { get; set; }

        [Field("WarningCount")]
        public int WarningCount { get; set; }

        [Field("Species")]
        public string Species { get; set; }

        [Field("LiveDead")]
        public string LiveDead { get; set; }

        [Field("EntryType")]
        public string EntryType { get; set; }

        [Field("TreeCount")]
        public int TreeCount { get; set; }

        [Field("Reason")]
        public string Reason { get; set; }

        [Field("TreeNumber", SourceName = "Tree_V3")]
        public int? TreeNumber { get; set; }

        [Field("TreeID")]
        public string TreeID { get; set; }

        [Field("TallyLedgerID")]
        public string TallyLedgerID { get; set; }
    }
}