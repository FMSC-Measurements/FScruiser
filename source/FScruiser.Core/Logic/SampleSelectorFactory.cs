using CruiseDAL.DataObjects;
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
        public static SampleSelecter MakeSampleSelecter(SampleGroup sampleGroup)
        {
            var method = sampleGroup.Method;

            if (method == "100"
                || method == "FIX"
                || method == "PNT"
                || method == "FIXCNT")
            { return null; }

            if (!string.IsNullOrEmpty(sampleGroup.SampleSelectorState))
            {
                var selector = InitializePersistedSampler(sampleGroup);
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
                        if (String.Equals(sampleGroup.SampleSelectorType, CruiseMethods.SYSTEMATIC_SAMPLER_TYPE))
                        {
                            return MakeSystematicSampleSelector(sampleGroup);
                        }
                        else if (String.Equals(sampleGroup.SampleSelectorType, CruiseMethods.CLICKER_SAMPLER_TYPE))
                        {
                            return new ClickerSelecter { Frequency = (int)sampleGroup.SamplingFrequency };
                        }
                        else
                        {
                            return MakeBlockSampleSelector(sampleGroup);
                        }
                    }
                case "3P":
                case "S3P":
                case "P3P":
                case "F3P":
                    {
                        return MakeThreePSampleSelector(sampleGroup);
                    }
                case "FCM":
                case "PCM":
                    {
                        return MakeSystematicSampleSelector(sampleGroup);
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
        public static SampleSelecter MakeThreePSampleSelector(SampleGroup sampleGroup)
        {
            SampleSelecter selecter = null;
            int iFrequency = (int)sampleGroup.InsuranceFrequency;
            int kz = (int)sampleGroup.KZ;
            int maxKPI = 100000;
            selecter = new FMSC.Sampling.ThreePSelecter(kz, maxKPI, iFrequency);
            return selecter;
        }

        public static SampleSelecter MakeSystematicSampleSelector(SampleGroup sampleGroup)
        {
            SampleSelecter selecter = null;
            int iFrequency = (int)sampleGroup.InsuranceFrequency;
            int frequency = (int)sampleGroup.SamplingFrequency;
            if (frequency == 0) { selecter = new ZeroFrequencySelecter(); }
            else
            {
                selecter = new FMSC.Sampling.SystematicSelecter(frequency, iFrequency, true);
            }
            return selecter;
        }

        public static SampleSelecter MakeBlockSampleSelector(SampleGroup sampleGroup)
        {
            SampleSelecter selecter = null;
            int iFrequency = (int)sampleGroup.InsuranceFrequency;
            int frequency = (int)sampleGroup.SamplingFrequency;
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
        private static bool ValidateFreqSelecter(SampleGroup sampleGroup, IFrequencyBasedSelecter freqSelecter)
        {
            //ensure sampler frequency matches sample group freqency
            if (freqSelecter != null
                && freqSelecter.Frequency != sampleGroup.SamplingFrequency
                || freqSelecter.ITreeFrequency != sampleGroup.InsuranceFrequency)
            {
                //older versions of FMSC.Sampling would use -1 instead of 0 if InsuranceFrequency was 0
                if (freqSelecter.ITreeFrequency == -1
                    && sampleGroup.InsuranceFrequency == 0)
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
        private static SampleSelecter InitializePersistedSampler(SampleGroup sampleGroup)
        {
            SampleSelecter selecter;
            try
            {
                selecter = DeserializeSamplerState(sampleGroup);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e, "Exception");
                /*keep calm and carry on*/
                return null;
            }

            if (selecter != null && selecter is IFrequencyBasedSelecter)
            {
                if (!ValidateFreqSelecter(sampleGroup, (IFrequencyBasedSelecter)selecter))
                {
                    string message = string.Format("Frequency missmatch on SG:{0} Sf={1} If={2}; SelectorState: Sf={3} If={4};",
                            sampleGroup.Code,
                            sampleGroup.SamplingFrequency,
                            sampleGroup.InsuranceFrequency,
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
        public static SampleSelecter DeserializeSamplerState(SampleGroup sampleGroup)
        {
            switch (sampleGroup.SampleSelectorType)
            {
                case "Block":
                case "BlockSelecter":
                    {
                        var serializer = new XmlSerializer(typeof(BlockSelecter));
                        using (var reader = new StringReader(sampleGroup.SampleSelectorState))
                        {
                            return (SampleSelecter)serializer.Deserialize(reader);
                        }
                    }
                case "SRS":
                case "SRSSelecter":
                    {
                        return new SRSSelecter((int)sampleGroup.SamplingFrequency, (int)sampleGroup.InsuranceFrequency);
                    }
                case "Systematic":
                case "SystematicSelecter":
                    {
                        var serializer = new XmlSerializer(typeof(SystematicSelecter));
                        using (var reader = new StringReader(sampleGroup.SampleSelectorState))
                        {
                            return (SampleSelecter)serializer.Deserialize(reader);
                        }
                    }
                case "ThreeP":
                case "ThreePSelecter":
                    {
                        return new ThreePSelecter((int)sampleGroup.KZ, 10000, (int)sampleGroup.InsuranceFrequency);
                    }
                default: { return null; }
            }
        }

        /// <exception cref="InvalidOperationException">An error occurred during serialization. The original exception is available
        ///using the System.Exception.InnerException property.</exception>
        public static void SerializeSamplerState(SampleGroup sampleGroup, SampleSelecter selector)
        {
            if (selector == null) { throw new ArgumentNullException(nameof(selector)); }
            if (sampleGroup == null) { throw new ArgumentNullException(nameof(sampleGroup)); }

            if (selector != null && (selector is BlockSelecter || selector is SystematicSelecter))
            {
                XmlSerializer serializer = new XmlSerializer(selector.GetType());
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, selector);
                sampleGroup.SampleSelectorState = writer.ToString();
                sampleGroup.SampleSelectorType = selector.GetType().Name;
            }
        }
    }
}