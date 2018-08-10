﻿using FMSC.ORM.EntityModel.Attributes;

namespace FScruiser.Models
{
    [EntitySource("Tree")]
    public class TreeStub_Plot : TreeStub
    {
        [Field(Alias = "CuttingUnitCode", SQLExpression = "CuttingUnit.Code")]
        public string CuttingUnitCode { get; set; }

        [Field(Alias = "PlotNumber", SQLExpression = "Plot.PlotNumber")]
        public int PlotNumber { get; set; }

        [Field("LiveDead")]
        public string LiveDead { get; set; }

        [Field("TreeCount")]
        public int TreeCount { get; set; }

        [Field("STM")]
        public string STM { get; set; }

        public bool IsSTM
        {
            get { return STM == "Y"; }
            set { STM = (value) ? "Y" : "N"; }
        }

        [Field("KPI")]
        public int KPI { get; set; }

        [Field(Name = "Initials")]
        public string Initials { get; set; }
    }
}