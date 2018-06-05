using CruiseDAL.DataObjects;
using CruiseDAL.Enums;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.Sampling;
using System;
using System.Xml.Serialization;

namespace FScruiser.Models
{
    public class SampleGroup : SampleGroupDO
    {
        [Field(SQLExpression = "Stratum.Code", Alias = "StratumCode", PersistanceFlags = PersistanceFlags.Never)]
        public string StratumCode { get; set; }

        public SampleSelecter Sampler { get; set; }

        public SampleSelecter SecondarySampler { get; set; }
    }
}