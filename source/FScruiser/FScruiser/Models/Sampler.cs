using Backpack.EntityModel.Attributes;
using FMSC.Sampling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FScruiser.Models
{
    [EntitySource(SourceName = "SampleGroup", JoinCommands = "JOIN Stratum USING (Stratum_CN)")]
    public class Sampler
    {
        public long SampleGroup_CN { get; set; }

        [Field(Alias = "SampleGroupCode", SQLExpression = "SampleGroup.Code")]
        public string SampleGroupCode { get; set; }

        [Field(Alias = "StratumCode", SQLExpression = "Stratum.Code")]
        public string StratumCode { get; set; }

        [Field(Alias = "CruiseMethod", SQLExpression = "Stratum.Method")]
        public string CruiseMethod { get; set; }

        public int SamplingFrequency { get; set; }

        public int InsuranceFrequency { get; set; }

        public int MinKPI { get; set; }

        public int MaxKPI { get; set; }

        public int KZ { get; set; }
        public string SampleSelectorType { get; set; }

        public string SampleSelectorState { get; set; }

        SampleSelecter _selector;

        public SampleSelecter Selector
        {
            get
            {
                if (_selector == null)
                {
                    _selector = MakeSampleSelecter(CruiseMethod);
                }
                return _selector;
            }
        }

        #region Sample Selector Methods

        public SampleItem NextItem()
        {
            return _selector.NextItem();
        }

        public void SerializeSamplerState()
        {
            if (_selector == null) { return; }
            SampleSelecter selector = _selector;
            if (selector != null && (selector is BlockSelecter || selector is SystematicSelecter))
            {
                XmlSerializer serializer = new XmlSerializer(selector.GetType());
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, selector);
                    SampleSelectorState = writer.ToString();
                    SampleSelectorType = selector.GetType().Name;
                }
            }
        }

        /// <summary>
        /// Deserializes the state of the sampler.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">An error occurred.</exception>
        /// <returns></returns>
        public SampleSelecter DeserializeSamplerState(string state, string type)
        {
            switch (type)
            {
                case "Block":
                case "BlockSelecter":
                    {
                        var serializer = new XmlSerializer(typeof(BlockSelecter));
                        return DeserializeSamplerState(state, serializer);
                    }
                case "Systematic":
                case "SystematicSelecter":
                    {
                        var serializer = new XmlSerializer(typeof(SystematicSelecter));
                        return DeserializeSamplerState(state, serializer);
                    }
                default: { return null; }
            }
        }

        SampleSelecter DeserializeSamplerState(string state, XmlSerializer serializer)
        {
            using (StringReader reader = new StringReader(state))
            {
                SampleSelecter sampler = (SampleSelecter)serializer.Deserialize(reader);
                return sampler;
            }
        }

        public SampleSelecter MakeSampleSelecter(string cruiseMethod)
        {
            switch (cruiseMethod)
            {
                case "100":
                case "FIX":
                case "PNT":
                case "FIXCNT":
                    { return null; }
                case "3P":
                    {
                        return new ThreePSelecter(KZ, 10000, InsuranceFrequency);
                    }
            }

            SampleSelecter selector = null;
            if (!string.IsNullOrWhiteSpace(SampleSelectorState))
            {
                selector = DeserializeSamplerState(SampleSelectorState, SampleSelectorType);
                if (!ValidateFreqSelecter(selector as IFrequencyBasedSelecter))
                {
                    throw new Exception("frequency mismatch");
                }
            }

            //if ((sg.TallyMethod & CruiseDAL.Enums.TallyMode.Manual) == CruiseDAL.Enums.TallyMode.Manual)
            //{
            //    return null;
            //}

            if (selector == null)
            {
                selector = MakeDefaultSelector(cruiseMethod);
            }
            return selector;
        }

        SampleSelecter MakeDefaultSelector(string method)
        {
            switch (method)
            {
                case "STR":
                    {
                        if (SampleSelectorType != null
                            && SampleSelectorType.Equals("SystematicSelecter", StringComparison.OrdinalIgnoreCase))
                        {
                            return MakeSystematicSampleSelector();
                        }
                        else
                        {
                            return MakeBlockSampleSelector();
                        }
                    }
                case "3P":
                case "S3P":
                case "P3P":
                case "F3P":
                    {
                        return MakeThreePSampleSelector();
                    }
                case "FCM":
                case "PCM":
                    {
                        return MakeSystematicSampleSelector();
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        SampleSelecter MakeThreePSampleSelector()
        {
            int maxKPI = 100000;
            return new FMSC.Sampling.ThreePSelecter(KZ, maxKPI, InsuranceFrequency);
        }

        SampleSelecter MakeSystematicSampleSelector()
        {
            if (SamplingFrequency == 0) { return null; }
            else
            {
                return new FMSC.Sampling.SystematicSelecter(SamplingFrequency, InsuranceFrequency, true);
            }
        }

        SampleSelecter MakeBlockSampleSelector()
        {
            if (SamplingFrequency == 0) { return null; }
            else
            {
                return new FMSC.Sampling.BlockSelecter(SamplingFrequency, InsuranceFrequency);
            }
        }

        bool ValidateFreqSelecter(IFrequencyBasedSelecter freqSelecter)
        {
            if (freqSelecter == null) { return true; }

            //ensure sampler frequency matches sample group frequency
            if (freqSelecter.Frequency != this.SamplingFrequency
                || freqSelecter.ITreeFrequency != this.InsuranceFrequency)
            {
                //older versions of FMSC.Sampling would use -1 instead of 0 if InsuranceFrequency was 0
                if (freqSelecter.ITreeFrequency == -1
                    && this.InsuranceFrequency == 0)
                { return true; }

                return false;
            }
            else { return true; }
        }

        #endregion Sample Selector Methods
    }
}