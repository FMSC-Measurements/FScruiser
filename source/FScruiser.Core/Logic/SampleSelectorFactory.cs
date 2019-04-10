using CruiseDAL.Schema;
using FMSC.Sampling;
using FScruiser.Models;
using System;
using System.IO;
using System.Xml.Serialization;

namespace FScruiser.Logic
{
    public class ClickerSelecter : SampleSelecter, IFrequencyBasedSelecter
    {
        public int Frequency { get; set; }

        public override SampleItem NextItem()
        {
            return new boolItem() { IsSelected = true };
        }

        public override bool Ready(bool throwException)
        {
            return true;
        }
    }

    public class ZeroFrequencySelecter : SampleSelecter, IFrequencyBasedSelecter
    {
        public int Frequency {
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

    public class FrequencyMismatchException : Exception
    {
        public FrequencyMismatchException(string message) : base(message)
        { }
    }

    public class SampleSelectorFactory
    {
        public static SampleSelecter MakeSampleSelecter(SamplerState samplerState)
        {
            var method = samplerState.Method;

            if (method == "100"
                || method == "FIX"
                || method == "PNT"
                || method == "FIXCNT")
            { return null; }

            if (!string.IsNullOrEmpty(samplerState.SampleSelectorState))
            {
                var selector = InitializePersistedSampler(samplerState);
                if (selector != null)
                { return selector; }
            }

            //if ((sg.TallyMethod & CruiseDAL.Enums.TallyMode.Manual) == CruiseDAL.Enums.TallyMode.Manual)
            //{
            //    return null;
            //}

            switch (method)
            {
                case "STR":
                    {
                        if (String.Equals(samplerState.SampleSelectorType, CruiseMethods.SYSTEMATIC_SAMPLER_TYPE))
                        {
                            return MakeSystematicSampleSelector(samplerState);
                        }
                        else if (String.Equals(samplerState.SampleSelectorType, CruiseMethods.CLICKER_SAMPLER_TYPE))
                        {
                            return new ClickerSelecter { Frequency = (int)samplerState.SamplingFrequency };
                        }
                        else
                        {
                            return MakeBlockSampleSelector(samplerState);
                        }
                    }
                case "3P":
                case "S3P":
                case "P3P":
                case "F3P":
                    {
                        return MakeThreePSampleSelector(samplerState);
                    }
                case "FCM":
                case "PCM":
                    {
                        return MakeSystematicSampleSelector(samplerState);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        /// <exception cref="FrequencyMismatchException">frequency on samplegroup and frequency on serialized sampler state are different</exception>
        //public SampleSelecter MakeSampleSelecter(SampleGroupDO sampleGroup, string stratumMethod)
        //{
        //    if (stratumMethod == CruiseMethods.H_PCT
        //        || stratumMethod == CruiseMethods.FIX
        //        || stratumMethod == CruiseMethods.PNT
        //        || stratumMethod == CruiseMethods.FIXCNT)
        //    { return null; }

        //    if (!string.IsNullOrEmpty(sampleGroup.SampleSelectorState))
        //    {
        //        var selector = InitializePersistedSampler(sampleGroup);
        //        if (selector != null)
        //        { return selector; }
        //    }
        public static SampleSelecter MakeThreePSampleSelector(SamplerState samplerState)
        {
            SampleSelecter selecter = null;
            int iFrequency = (int)samplerState.InsuranceFrequency;
            int kz = (int)samplerState.KZ;
            int maxKPI = 100000;
            selecter = new FMSC.Sampling.ThreePSelecter(kz, maxKPI, iFrequency);
            return selecter;
        }

        public static SampleSelecter MakeSystematicSampleSelector(SamplerState samplerState)
        {
            SampleSelecter selecter = null;
            int iFrequency = (int)samplerState.InsuranceFrequency;
            int frequency = (int)samplerState.SamplingFrequency;
            if (frequency == 0) { selecter = new ZeroFrequencySelecter(); }
            else
            {
                selecter = new FMSC.Sampling.SystematicSelecter(frequency, iFrequency, true);
            }
            return selecter;
        }

        public static SampleSelecter MakeBlockSampleSelector(SamplerState samplerState)
        {
            SampleSelecter selecter = null;
            int iFrequency = (int)samplerState.InsuranceFrequency;
            int frequency = (int)samplerState.SamplingFrequency;
            if (frequency == 0) { selecter = null; }
            else
            {
                selecter = new FMSC.Sampling.BlockSelecter(frequency, iFrequency);
            }
            return selecter;
        }

        //    //if ((sg.TallyMethod & CruiseDAL.Enums.TallyMode.Manual) == CruiseDAL.Enums.TallyMode.Manual)
        //    //{
        //    //    return null;
        //    //}
        private static bool ValidateFreqSelecter(SamplerState samplerState, IFrequencyBasedSelecter freqSelecter)
        {
            //ensure sampler frequency matches sample group freqency
            if (freqSelecter != null
                && freqSelecter.Frequency != samplerState.SamplingFrequency
                || freqSelecter.ITreeFrequency != samplerState.InsuranceFrequency)
            {
                //older versions of FMSC.Sampling would use -1 instead of 0 if InsuranceFrequency was 0
                if (freqSelecter.ITreeFrequency == -1
                    && samplerState.InsuranceFrequency == 0)
                { return true; }

                return false;
            }
            else { return true; }
        }

        //    switch (stratumMethod)
        //    {
        //        case "STR":
        //            {
        //                if (String.Equals(sampleGroup.SampleSelectorType, CruiseMethods.SYSTEMATIC_SAMPLER_TYPE))
        //                {
        //                    return MakeSystematicSampleSelector(sampleGroup);
        //                }
        //                else if (String.Equals(sampleGroup.SampleSelectorType, CruiseMethods.CLICKER_SAMPLER_TYPE))
        //                {
        //                    return new ClickerSelecter();
        //                }
        //                else
        //                {
        //                    return MakeBlockSampleSelector(sampleGroup);
        //                }
        //            }
        //        case "3P":
        //        case "S3P":
        //        case "P3P":
        //        case "F3P":
        //            {
        //                return MakeThreePSampleSelector(sampleGroup);
        //            }
        //        case "FCM":
        //        case "PCM":
        //            {
        //                return MakeSystematicSampleSelector(sampleGroup);
        //            }
        //        default:
        //            {
        //                return null;
        //            }
        //    }
        //}
        private static SampleSelecter InitializePersistedSampler(SamplerState samplerState)
        {
            SampleSelecter selecter;
            try
            {
                selecter = DeserializeSamplerState(samplerState);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e, "Exception");
                /*keep calm and carry on*/
                return null;
            }

            if (selecter != null && selecter is IFrequencyBasedSelecter)
            {
                if (!ValidateFreqSelecter(samplerState, (IFrequencyBasedSelecter)selecter))
                {
                    string message = string.Format("Frequency missmatch on SG:{0} Sf={1} If={2}; SelectorState: Sf={3} If={4};",
                            samplerState.SampleGroupCode,
                            samplerState.SamplingFrequency,
                            samplerState.InsuranceFrequency,
                            ((FMSC.Sampling.IFrequencyBasedSelecter)selecter).Frequency,
                            ((FMSC.Sampling.IFrequencyBasedSelecter)selecter).ITreeFrequency);

                    throw new FrequencyMismatchException(message);

                    //DAL.LogMessage(message, "I");

                    //if (CanEditSampleGroup())
                    //{
                    //    SampleSelectorState = null;
                    //    return null;
                    //}
                    //else
                    //{
                    //    throw new UserFacingException("Oops! Sample Frequency on sample group " +
                    //        this.Code + " has been modified.\r\n If you are trying to change the sample freqency during a cruise, you should create a new sample group.", (Exception)null);
                    //}
                }
            }
            return selecter;
        }

        /// <exception cref="InvalidOperationException">An error occurred during deserialization. The original exception is available
        ///using the System.Exception.InnerException property.</exception>
        public static SampleSelecter DeserializeSamplerState(SamplerState samplerState)
        {
            switch (samplerState.SampleSelectorType)
            {
                case "Block":
                case "BlockSelecter":
                    {
                        var serializer = new XmlSerializer(typeof(BlockSelecter));
                        using (var reader = new StringReader(samplerState.SampleSelectorState))
                        {
                            return (SampleSelecter)serializer.Deserialize(reader);
                        }
                    }
                case "SRS":
                case "SRSSelecter":
                    {
                        return new SRSSelecter((int)samplerState.SamplingFrequency, (int)samplerState.InsuranceFrequency);
                    }
                case "Systematic":
                case "SystematicSelecter":
                    {
                        var serializer = new XmlSerializer(typeof(SystematicSelecter));
                        using (var reader = new StringReader(samplerState.SampleSelectorState))
                        {
                            return (SampleSelecter)serializer.Deserialize(reader);
                        }
                    }
                case "ThreeP":
                case "ThreePSelecter":
                    {
                        return new ThreePSelecter((int)samplerState.KZ, 10000, (int)samplerState.InsuranceFrequency);
                    }
                default: { return null; }
            }
        }

        /// <exception cref="InvalidOperationException">An error occurred during serialization. The original exception is available
        ///using the System.Exception.InnerException property.</exception>
        public static void SerializeSamplerState(SamplerState samplerState, SampleSelecter selector)
        {
            if (selector == null) { throw new ArgumentNullException(nameof(selector)); }
            if (samplerState == null) { throw new ArgumentNullException(nameof(samplerState)); }

            if (selector != null && (selector is BlockSelecter || selector is SystematicSelecter))
            {
                XmlSerializer serializer = new XmlSerializer(selector.GetType());
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, selector);
                samplerState.SampleSelectorState = writer.ToString();
                samplerState.SampleSelectorType = selector.GetType().Name;
            }
        }
    }
}