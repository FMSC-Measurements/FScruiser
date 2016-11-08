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

            var sg = new SampleGroup { SamplingFrequency = freq };
            var sampler = new Sampler(sg);

            var selector = sampler.MakeSampleSelecter(CruiseMethods.STR);

            selector.Should().NotBeNull();
            selector.As<IFrequencyBasedSelecter>().Frequency.Should().Be(freq);
            selector.Should().BeOfType<BlockSelecter>();
        }

        [Fact]
        public void TestMakeSampleSelecter_STR_BLOCK()
        {
            var freq = 10;

            var sg = new SampleGroup { SamplingFrequency = freq, SampleSelectorType = Sampler.BLOCK_SELECTER };
            var sampler = new Sampler(sg);

            sampler.SerializeSamplerState(new BlockSelecter(sampler.SamplingFrequency, sampler.InsuranceFrequency), sg);

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

            var sg = new SampleGroup { SamplingFrequency = freq, SampleSelectorType = Sampler.SYSTEMATIC_SELECTER };
            var sampler = new Sampler(sg);

            sampler.SerializeSamplerState(new SystematicSelecter(sampler.SamplingFrequency, sampler.InsuranceFrequency, true), sg);

            sampler.SampleSelectorState.Should().NotBeNullOrEmpty();

            var selector = sampler.MakeSampleSelecter(CruiseMethods.STR);

            selector.Should().NotBeNull();
            selector.As<IFrequencyBasedSelecter>().Frequency.Should().Be(freq);
            selector.Should().BeOfType<SystematicSelecter>();
        }
    }
}