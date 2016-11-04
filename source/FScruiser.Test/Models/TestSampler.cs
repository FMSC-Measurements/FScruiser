using FluentAssertions;
using FMSC.Sampling;
using FScruiser.Models;
using System;
using System.Collections.Generic;

//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FScruiser.Test.Models
{
    public class TestSampler
    {
        [Fact]
        public void TestMakeSampleSelecter_STR_DEFAULT()
        {
            var freq = 10;

            var sampler = new Sampler()
            {
                SamplingFrequency = freq,
            };

            var selector = sampler.MakeSampleSelecter(CruiseMethods.STR);

            selector.Should().NotBeNull();
            selector.As<IFrequencyBasedSelecter>().Frequency.Should().Be(freq);
            selector.Should().BeOfType<BlockSelecter>();
        }

        [Fact]
        public void TestMakeSampleSelecter_STR_BLOCK()
        {
            var freq = 10;

            var sampler = new Sampler()
            {
                SampleSelectorType = Sampler.BLOCK_SELECTER,
                SamplingFrequency = freq,
            };

            sampler.SerializeSamplerState(new BlockSelecter(sampler.SamplingFrequency, sampler.InsuranceFrequency));

            sampler.SampleSelectorState.Should().NotBeNullOrEmpty();

            var selector = sampler.MakeSampleSelecter(CruiseMethods.STR);

            selector.Should().NotBeNull();
            selector.As<IFrequencyBasedSelecter>().Frequency.Should().Be(freq);
            selector.Should().BeOfType<BlockSelecter>();
        }

        [Fact]
        public void TestMakeSampleSelecter_STR_SYSTEMATIC()
        {
            var freq = 10;

            var sampler = new Sampler()
            {
                SampleSelectorType = Sampler.SYSTEMATIC_SELECTER,
                SamplingFrequency = freq,
            };

            sampler.SerializeSamplerState(new SystematicSelecter(sampler.SamplingFrequency, sampler.InsuranceFrequency, true));

            sampler.SampleSelectorState.Should().NotBeNullOrEmpty();

            var selector = sampler.MakeSampleSelecter(CruiseMethods.STR);

            selector.Should().NotBeNull();
            selector.As<IFrequencyBasedSelecter>().Frequency.Should().Be(freq);
            selector.Should().BeOfType<SystematicSelecter>();
        }
    }
}