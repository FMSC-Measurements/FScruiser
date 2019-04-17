using FMSC.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Logic
{
    public class ClickerSelecter : SampleSelecter, IFrequencyBasedSelecter
    {
        public int Frequency { get; set; }

        public override SampleItem NextItem()
        {
            return new boolItem() { IsSelected = true };
        }

        public override bool Ready(bool throwException)
        {
            return true;
        }
    }
}
