using CruiseDAL.DataObjects;
using CruiseDAL.Enums;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.Sampling;
using System;
using System.Xml.Serialization;

namespace FScruiser.Models
{
    [EntitySource("SampleGroup")]
    public class SampleGroup
    {
        [Field(Name = "Code")]
        public string Code { get; set; }

        [Field(Name = "SamplingFrequency")]
        public long SamplingFrequency { get; set; }

        [Field(Name = "InsuranceFrequency")]
        public long InsuranceFrequency { get; set; }

        [Field(Name = "KZ")]
        public long KZ { get; set; }

        [Field(Name = "BigBAF")]
        public float BigBAF { get; set; }

        [Field(Name = "SmallFPS")]
        public float SmallFPS { get; set; }

        [Field(Name = "Description")]
        public string Description { get; set; }

        [Field(Name = "SampleSelectorType")]
        public string SampleSelectorType { get; set; }

        [Field(Name = "SampleSelectorState")]
        public string SampleSelectorState { get; set; }

        [Field(Name = "MinKPI")]
        public long MinKPI { get; set; }

        [Field(Name = "MaxKPI")]
        public virtual long MaxKPI { get; set; }

        [Field(SQLExpression = "Stratum.Code", Alias = "StratumCode", PersistanceFlags = PersistanceFlags.Never)]
        public string StratumCode { get; set; }

        [Field(SQLExpression = "Stratum.Method", Alias = "Method", PersistanceFlags = PersistanceFlags.Never)]
        public string Method { get; set; }
    }
}