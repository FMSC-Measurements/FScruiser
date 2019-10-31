using FluentAssertions;
using FScruiser.Core.Test.Services;
using FScruiser.Data;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FScruiser.Core.Test.Data
{
    public class SamplerInfoDataservice_Tests : Datastore_TestBase
    {
        public SamplerInfoDataservice_Tests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory]
        [InlineData("st1", "sg1")]
        public void GetSamplerInfo(string stratumCode, string sampleGroupCode)
        {
            using (var database = base.CreateDatabase())
            {
                var ds = new SamplerInfoDataservice(database, new TestDeviceInfoService());


                ds.GetSamplerInfo(stratumCode, sampleGroupCode);
            }


        }

        [Theory]
        [InlineData("st1", "sg1")]
        public void GetSamplerState(string stratumCode, string sampleGroupCode)
        {
            using (var database = base.CreateDatabase())
            {
                var ds = new SamplerInfoDataservice(database, new TestDeviceInfoService());


                ds.GetSamplerState(stratumCode, sampleGroupCode);
            }
        }

        [Theory]
        [InlineData("st1", "sg1")]
        public void UpsertSamplerState(string stratumCode, string sampleGroupCode)
        {
            using (var database = base.CreateDatabase())
            {
                var ds = new SamplerInfoDataservice(database, new TestDeviceInfoService());


                var ss = new SamplerState()
                {
                    StratumCode = stratumCode,
                    SampleGroupCode = sampleGroupCode,
                    BlockState = "something",
                };

                ds.UpsertSamplerState(ss);

                var ssAgain = ds.GetSamplerState(stratumCode, sampleGroupCode);

                ssAgain.Should().BeEquivalentTo(ss);
            }
        }
    }
}
