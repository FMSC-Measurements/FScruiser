using FMSC.ORM.EntityModel.Attributes;
using FScruiser.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace FScruiser.Models
{
    [EntitySource("CountTree")]
    public class CountTree : INPC_Base
    {
        int _treeCount;
        int _sumKPI;

        [PrimaryKeyField(Name = "CountTree_CN")]
        public int CountTree_CN { get; set; }

        [Field("TreeCount")]
        public int TreeCount
        {
            get { return _treeCount; }
            set { SetValue(ref _treeCount, value); }
        }

        [Field("SumKPI")]
        public int SumKPI
        {
            get { return _sumKPI; }
            set { SetValue(ref _sumKPI, value); }
        }

        [Field(Alias = "UnitCode", SQLExpression = "CuttingUnit.Code", PersistanceFlags = PersistanceFlags.Never)]
        public string UnitCode { get; set; }

        [Field(Alias = "StratumCode", SQLExpression = "Stratum.Code", PersistanceFlags = PersistanceFlags.Never)]
        public string StratumCode { get; set; }

        [Field(Alias = "SampleGroupCode", SQLExpression = "SampleGroup.Code", PersistanceFlags = PersistanceFlags.Never)]
        public string SampleGroupCode { get; set; }

        [Field(Alias = "Species", SQLExpression = "TreeDefaultValue.Species", PersistanceFlags = PersistanceFlags.Never)]
        public string Species { get; set; }


        [Field("CuttingUnit_CN")]
        public long CuttingUnit_CN { get; set; }

        [Field("SampleGroup_CN")]
        public long SampleGroup_CN { get; set; }

        [Field("TreeDefaultValue_CN")]
        public long? TreeDefaultValue_CN { get; set; }

    }
}
