﻿using FMSC.ORM.EntityModel.Attributes;
using System;

namespace FScruiser.Models
{
    [EntitySource("TallyLedger")]
    public class TallyAction
    {
        public enum CountOrMeasureValue { M, C, I };

        public TallyAction()
        {
        }

        public TallyAction(TallyPopulation population)
        {
            StratumCode = population.StratumCode;
            SampleGroupCode = population.SampleGroupCode;
            Species = population.Species;
            LiveDead = population.LiveDead;
        }

        public TallyAction(string unitCode, TallyPopulation population) : this(population)
        {
            CuttingUnitCode = unitCode;
        }

        public TallyAction(string unitCode, int plotNumber, TallyPopulation population) : this(unitCode, population)
        {
            PlotNumber = plotNumber;
        }

        [Field(nameof(CuttingUnitCode))]
        public string CuttingUnitCode { get; set; }

        [Field(nameof(PlotNumber))]
        public int? PlotNumber { get; set; }

        [Field(nameof(StratumCode))]
        public string StratumCode { get; set; }

        [Field(nameof(SampleGroupCode))]
        public string SampleGroupCode { get; set; }

        [Field(nameof(Species))]
        public string Species { get; set; }

        [Field(nameof(LiveDead))]
        public string LiveDead { get; set; }

        [Field(nameof(TreeCount))]
        public int TreeCount { get; set; }

        [Field(nameof(KPI))]
        public int KPI { get; set; }

        [Field("STM")]
        public bool STM { get; set; }

        public bool IsSample { get; set; }

        public bool IsInsuranceSample { get; set; }

        public CountOrMeasureValue CountOrMeasure { get; set; }

        //random number generated by the three p sample selector,
        //for debug purposis I think, it was Matt's idea.
        //If they ever ask for this plot based three p methods just say no
        [Field(nameof(ThreePRandomValue))]
        public int ThreePRandomValue { get; set; }

        [Field(nameof(EntryType))]
        public string EntryType => TallyLedger.EntryTypeValues.TALLY;

        [Field("Initials")]
        public string Initials { get; set; }
    }
}