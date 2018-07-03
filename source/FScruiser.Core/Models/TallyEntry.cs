using FMSC.ORM.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("TallyLedger")]
    public class TallyEntry
    {
        private long? _treeNumber;

        public TallyEntry() { }

        public TallyEntry(string unitCode, TallyPopulation population)
        {
            UnitCode = unitCode;

            StratumCode = population.StratumCode;
            SampleGroupCode = population.SampleGroupCode;
            Species = population.Species;
            LiveDead = population.LiveDead;
        }

        [Field(nameof(TallyLedgerID))]
        public string TallyLedgerID { get; set; }

        [Field(nameof(UnitCode))]
        public string UnitCode { get; set; }
        [Field(nameof(PlotNumber))]
        public int? PlotNumber { get; set; }

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
        

        [Field(nameof(Tree_GUID))]
        public string Tree_GUID { get; set; }

        [Field(nameof(TimeStamp))]
        public DateTime TimeStamp { get; set; }
        //[Field(nameof(Signature))]
        //public string Signature { get; set; }



        #region readonly props
        [Field(Alias ="TreeNumber", SQLExpression = "Tree.TreeNumber", PersistanceFlags = PersistanceFlags.Never)]
        public long? TreeNumber { get; set; }

        [Field(Alias = "CountOrMeasure", SQLExpression = "Tree.CountOrMeasure", PersistanceFlags = PersistanceFlags.Never)]
        public string CountOrMeasure { get; set; }

        [Field(Alias ="IsSTM", SQLExpression = "Tree.STM = 'Y'", PersistanceFlags = PersistanceFlags.Never)]
        public bool IsSTM { get; set; }

        [Field(Alias = "Initials", SQLExpression = "Tree.Initials", PersistanceFlags = PersistanceFlags.Never)]
        public string Initials { get; set; }

        public bool HasTree => !string.IsNullOrWhiteSpace(Tree_GUID) && Tree_GUID != Guid.Empty.ToString();

        public string TallyDescription => SampleGroupCode + (string.IsNullOrWhiteSpace(Species) ? "" : $"-{Species}") ;
        #endregion
    }
}
