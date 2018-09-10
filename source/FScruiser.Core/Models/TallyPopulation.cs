﻿using CruiseDAL.Schema;
using FMSC.ORM.EntityModel.Attributes;
using FMSC.Sampling;
using FScruiser.Util;
using System.Linq;

namespace FScruiser.Models
{
    [EntitySource("CountTree")]
    public class TallyPopulation : TallyPopulation_Base
    {
        int _treeCount;
        int _sumKPI;

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
    }
}