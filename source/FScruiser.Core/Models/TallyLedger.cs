using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FScruiser.Constants;

namespace FScruiser.Models
{
    [EntitySource("TallyLedger")]
    public class TallyLedger
    {
        public TallyLedger(string unitCode, TallyPopulation tallyPopulation)
        {
            UnitCode = unitCode;
            StratumCode = tallyPopulation.StratumCode;
            SampleGroupCode = tallyPopulation.SampleGroupCode;
            Species = tallyPopulation.Species ?? "";
            LiveDead = tallyPopulation.LiveDead ?? "";
        }

        [Field(nameof(TallyLedgerID))]
        public string TallyLedgerID { get; set; }

        [Field(nameof(UnitCode))]
        public string UnitCode { get; set; }

        [Field(nameof(StratumCode))]
        public string StratumCode { get; set; }

        [Field(nameof(SampleGroupCode))]
        public string SampleGroupCode { get; set; }

        [Field(nameof(Species))]
        public string Species { get; set; }

        [Field(nameof(LiveDead))]
        public string LiveDead { get; set; }

        [Field(nameof(TreeCount))]
        public int TreeCount { get; set; }

        [Field(nameof(KPI))]
        public int KPI { get; set; }

        [Field(nameof(ThreePRandomValue))]
        public int ThreePRandomValue { get; set; }

        [Field(nameof(Tree_GUID))]
        public string Tree_GUID { get; set; }

        [Field(nameof(TimeStamp))]
        public DateTime TimeStamp { get; set; }

        [Field(nameof(Reason))]
        public string Reason { get; set; }

        [Field(nameof(Signature))]
        public string Signature { get; set; }

        [Field(nameof(Remarks))]
        public string Remarks { get; set; }

        [Field(nameof(EntryType))]
        public TallyLedger_EntryType EntryType { get; set; }
    }
}
