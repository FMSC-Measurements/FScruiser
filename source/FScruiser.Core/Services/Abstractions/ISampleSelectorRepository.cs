﻿using FMSC.Sampling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Services
{
    public interface ISampleSelectorDataService
    {
        IEnumerable<SampleSelecter> GetSamplersBySampleGroupCode(string stratumCode, string sgCode);

        void SaveSamplerStates();
    }
}
