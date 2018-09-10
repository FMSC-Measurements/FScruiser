using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMSC.Sampling;
using FScruiser.Logic;

namespace FScruiser.Services
{
    public class SampleSelectorRepository : ISampleSelectorDataService
    {
        private Dictionary<string, SampleSelecter[]> _sampleSelectors = new Dictionary<string, SampleSelecter[]>();

        public SampleSelectorRepository(ICuttingUnitDatastore datastore)
        {
            Datastore = datastore ?? throw new ArgumentNullException(nameof(datastore));
        }

        public ICuttingUnitDatastore Datastore { get; set; }

        public IEnumerable<SampleSelecter> GetSamplersBySampleGroupCode(string stratumCode, string sgCode)
        {
            var key = stratumCode + "/" + sgCode;

            if (_sampleSelectors.ContainsKey(key) == false)
            {
                var sampleGroup = Datastore.GetSampleGroup(stratumCode, sgCode);
                var samplers = new SampleSelecter[2];

                samplers[0] = SampleSelectorFactory.MakeSampleSelecter(sampleGroup);
                if (sampleGroup.Method == CruiseDAL.Schema.CruiseMethods.S3P)
                {
                    samplers[1] = SampleSelectorFactory.MakeSystematicSampleSelector(sampleGroup);
                }

                _sampleSelectors.Add(key, samplers);
            }

            return _sampleSelectors[key];
        }

        public void SaveSamplerStates()
        {
            foreach(var sampler in _sampleSelectors.Values.SelectMany(x => x))
            {
                if(sampler is BlockSelecter blockSelecter)
                {
                    
                }
            }
        }
    }
}
