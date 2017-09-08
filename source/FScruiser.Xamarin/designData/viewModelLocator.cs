using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FScruiser.XF.DesignData
{
    public static class ViewModelLocator
    {
        static TallyPopulation _tallyPopulation;
        public static TallyPopulation TallyPopulation => _tallyPopulation ??
            (_tallyPopulation = MakeTallyPopulation());

        private static TallyPopulation MakeTallyPopulation()
        {
            var st = new Stratum() { Code = "St" };
            var sg = new SampleGroup() { Code = "Sg", Stratum = st };
            var tally = new Tally() { Description = $"{sg.Code}/{st.Code}" };

            return new TallyPopulation() { SampleGroup = sg, Tally = tally, TreeCount = 100 };
        }
    }
}
