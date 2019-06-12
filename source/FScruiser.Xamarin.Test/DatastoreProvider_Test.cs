using CruiseDAL;
using FluentAssertions;
using FScruiser.Services;
using FScruiser.XF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FScruiser.XF
{
    public class DatastoreProvider_Test
    {
        [Theory]
        [InlineData(typeof(ICuttingUnitDatastore), typeof(CuttingUnitDatastore))]
        [InlineData(typeof(IPlotDatastore), typeof(CuttingUnitDatastore))]
        [InlineData(typeof(ITreeDatastore), typeof(CuttingUnitDatastore))]
        [InlineData(typeof(ISampleSelectorDataService), typeof(SampleSelectorRepository))]
        public void Get_Test(Type typeIn, Type typeExpected)
        {
            var cruiseDatastore = new CruiseDatastore_V3();
            var cuDatastore = new CuttingUnitDatastore(cruiseDatastore);
            var samplerDatastore = new SampleSelectorRepository(cuDatastore);

            var datastoreProvider = new DatastoreProvider()
            {
                CruiseDatastore = cruiseDatastore,
                CuttingUnitDatastore = cuDatastore,
                SampleSelectorDataService = samplerDatastore,
            };

            var result = datastoreProvider.Get(typeIn);
            result.Should().BeOfType(typeExpected);
        }
    }
}
