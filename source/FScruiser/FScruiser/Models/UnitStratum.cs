﻿using Backpack.EntityModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Models
{
    [EntitySource("CuttingUnitStratum",
        JoinCommands = "JOIN CuttingUnit USING (CuttingUnit_CN) JOIN Stratum USING (Stratum_CN)")]
    public class UnitStratum
    {
        [Field(Alias = "CuttingUnitCode", SQLExpression = "CuttingUnit.Code")]
        public string CuttingUnitCode { get; set; }

        public long? CuttingUnit_CN { get; set; }

        [Field(Alias = "StratumCode", SQLExpression = "Stratum.Code")]
        public string StratumCode { get; set; }

        public long? Stratum_CN { get; set; }
    }
}