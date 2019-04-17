using FMSC.Sampling;
using System;

namespace FScruiser.Logic
{
    public class HundredPCTSelector : SampleSelecter, IFrequencyBasedSelecter
    {
        public int Frequency
        {
            get => 1;
            set => throw new InvalidOperationException();
        }

        public override SampleItem NextItem()
        {
            return new boolItem(1, false, true);
        }

        public override bool Ready(bool throwException)
        {
            return true;
        }
    }
}