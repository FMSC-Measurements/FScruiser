using FluentAssertions;
using FMSC.Sampling;
using FScruiser.Logic;
using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FScruiser.Core.Test.Logic
{
    public class SampleSelectorFactory_test
    {
        [Fact]
        public void MakeSampleSelecter_100PCT()
        {
            var sg = new SamplerState()
            {
                Method = "100PCT",
                SampleSelectorState = "something",
                SampleSelectorType = "something"
            };

            sg.SampleSelectorState = null;

            var sampler = SampleSelectorFactory.MakeSampleSelecter(sg);
            sampler.Should().BeNull();
        }

        [Fact]
        public void MakeSampleSelecter_STR_freq_1_and_sampleSelector_is_null()
        {
            //test: if sampling freq is > 0
            //AND SampleSelectorType is not defined
            //THEN Sampler is not null
            //AND is of type Blocked
            var sg = new SamplerState()
            {
                Method = "STR",
                SamplingFrequency = 1,
                SampleSelectorState = "something"
            };

            var sampler = SampleSelectorFactory.MakeSampleSelecter(sg);
            sampler.Should().NotBeNull();
            sampler.GetType().Should().Be(typeof(BlockSelecter));

            var blockSampler = sampler as FMSC.Sampling.BlockSelecter;
            blockSampler.Frequency.Should().Be(1);
        }

        [Fact]
        public void MakeSampleSelecter_STR_freq_1_and_type_is_systematic()
        {
            //test: if sampling freq is > 0
            //AND SampleSelectorType is Systematic
            //THEN Sampler is not null
            //AND is of type Systematic
            var sg = new SamplerState()
            {
                Method = "STR",
                SamplingFrequency = 1,
                SampleSelectorType = "SystematicSelecter",
                SampleSelectorState = "something"
            };

            var sampler = SampleSelectorFactory.MakeSampleSelecter(sg);

            sampler.Should().NotBeNull();

            sampler.GetType().Should().Be(typeof(SystematicSelecter));

            var systmaticSampler = sampler as FMSC.Sampling.SystematicSelecter;
            systmaticSampler.Frequency.Should().Be(1);
        }

        [Fact]
        public void MakeSampleSelecter_STR_freq_0()
        {
            //test: if sampling freq is 0
            //then Sampler is null
            var sg = new SamplerState()
            {
                Method = "STR",
                SamplingFrequency = 0,
                SampleSelectorState = "something"
            };

            var sampler = SampleSelectorFactory.MakeSampleSelecter(sg);

            sampler.Should().BeNull();


        }

        [Fact]
        public void MakeSampleSelecter_STR_Clicker()
        {
            //test: if sampling freq is 0
            //then Sampler is null
            var sg = new SamplerState()
            {
                Method = "STR",
                SamplingFrequency = 20,
                SampleSelectorType = CruiseDAL.Schema.CruiseMethods.CLICKER_SAMPLER_TYPE,
                SampleSelectorState = "something"
            };

            var sampler = SampleSelectorFactory.MakeSampleSelecter(sg);
            sampler.Should().NotBeNull();

            sampler.Next().Should().BeTrue(); //clicker selector should always return true
            sampler.NextItem().IsSelected.Should().BeTrue();
        }

        [Theory]
        [InlineData("FCM")]
        [InlineData("PCM")]
        public void MakeSampleSelecter_multiStagePlot_freq_0(string method)
        {
            //test: if sampling freq is 0
            //then Sampler is null
            var sg = new SamplerState()
            {
                Method = method,
                SamplingFrequency = 0
            };

            var sampler = SampleSelectorFactory.MakeSampleSelecter(sg);

            sampler.Should().NotBeNull();
            sampler.NextItem().Should().BeNull();
            sampler.Should().BeOfType<ZeroFrequencySelecter>();
        }

        [Theory]
        [InlineData("FCM")]
        [InlineData("PCM")]
        public void MakeSampleSelecter_multiStagePlot(string method)
        {
            //test: if sampling freq is > 0
            //AND SampleSelectorType is not defined
            //THEN Sampler is not null
            //AND is of type Systematic
            var sg = new SamplerState()
            {
                Method = method,
                SamplingFrequency = 1,
                InsuranceFrequency = 1
            };

            var sampler = SampleSelectorFactory.MakeSampleSelecter(sg);

            sampler.Should().NotBeNull();
            sampler.Should().BeOfType<SystematicSelecter>();

            var freqSampler = sampler as FMSC.Sampling.IFrequencyBasedSelecter;
            freqSampler.Frequency.Should().Be(1);
            freqSampler.ITreeFrequency.Should().Be(1);
        }

        [Fact]
        public void DeserializeSamplerStatet_3P()
        {
            var sg = new SamplerState()
            {
                Method = "3P",
                SampleSelectorState = null,
                KZ = 100
            };

            var sampler = SampleSelectorFactory.MakeSampleSelecter(sg);

            sampler.Should().NotBeNull();
            sampler.Should().BeOfType<ThreePSelecter>();
            ((ThreePSelecter)sampler).KZ.Should().Be(100);
        }

        [Fact]
        public void MakeSampleSelecter_F3P()
        {
            var sg = new SamplerState()
            {
                Method = "F3P",
                SampleSelectorState = null,
                KZ = 100
            };

            var sampler = SampleSelectorFactory.MakeSampleSelecter(sg);
            sampler.Should().NotBeNull();
            sampler.Should().BeOfType<ThreePSelecter>();
            ((ThreePSelecter)sampler).KZ.Should().Be(100);
        }

        [Fact]
        public void MakeSampleSelecter_P3P()
        {
            var sg = new SamplerState()
            {
                Method = "P3P",
                SampleSelectorState = null,
                KZ = 100
            };

            var sampler = SampleSelectorFactory.MakeSampleSelecter(sg);
            sampler.Should().NotBeNull();
            sampler.Should().BeOfType<ThreePSelecter>();
            ((ThreePSelecter)sampler).KZ.Should().Be(100);
        }
    }
}
