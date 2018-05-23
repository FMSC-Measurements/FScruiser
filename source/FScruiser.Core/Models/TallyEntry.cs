using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    public class TallyEntry
    {
        public TallyEntry() { }

        public TallyEntry(TallyPopulation population)
        {
            UnitCode = population.Count.UnitCode;
            StratumCode = population.StratumCode;
            SGCode = population.SampleGroupCode;
            Species = population.Species;
        }

        public Guid TallyEntryID { get; set; }

        public string UnitCode { get; set; }
        public string StratumCode { get; set; }
        public string SGCode { get; set; }

        //TODO nullable
        public string Species { get; set; }

        public int? PlotNumber { get; set; }
        public int? TreeNumber { get; set; } 

        public bool IsSTM { get; set; }

        public int TreeCount { get; set; }
        public int KPI { get; set; }

        public string Signature { get; set; }

        public DateTime TimeStamp { get; set; }

        public Tree Tree { get; set; }

        #region readonly props
        public bool HasTree => TreeNumber != null;

        public string Initials => Tree?.Initials;

        public string TallyDescription => SGCode + (string.IsNullOrWhiteSpace(Species) ? "" : $"-{Species}") ;
        #endregion

        public void SetTree(Tree tree)
        {
            if (tree != null)
            {
                Tree = tree;
                TreeNumber = (int)tree.TreeNumber;
            }
        }
    }
}
