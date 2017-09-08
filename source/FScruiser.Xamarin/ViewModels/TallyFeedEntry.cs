using System;
using System.Collections.Generic;
using System.Text;
using FScruiser.Models;
using FScruiser.Core.Legacy.Models;

namespace FScruiser.XF.ViewModels
{
    public class TallyFeedEntry
    {
        public string Initials => Tree?.Initials;

        public int KPI => Tree?.KPI ?? 0;

        public int TreeNumber => Tree?.TreeNumber ?? 0;

        public string PopulationCode => (Count != null) ? $"{Count.SampleGroup.Stratum.Code}/{Count.SampleGroup.Code}" : "";

        public Tree Tree => TallyAction.TreeRecord;

        public CountTree Count => TallyAction?.Count;
        
        public TallyAction TallyAction { get; set; }

    }
}
