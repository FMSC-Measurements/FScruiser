using FMSC.Sampling;
using System;

namespace FScruiser.Logic
{
    public class ZeroFrequencySelecter : SampleSelecter, IFrequencyBasedSelecter
    {
        public int Frequency
        {
            get => 0;
            set => throw new InvalidOperationException();
        }

        public override SampleItem NextItem()
        {
            return (boolItem)null;
        }

        public override bool Ready(bool throwException)
        {
            return true;
        }
    }
}