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
        public SampleSelecter Sampler { get; set; }
    }
}