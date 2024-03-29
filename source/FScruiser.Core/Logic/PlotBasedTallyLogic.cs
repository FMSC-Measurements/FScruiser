﻿using CruiseDAL.Schema;
using FMSC.Sampling;
using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Logic
{
    public class PlotBasedTallyLogic
    {
        static readonly string[] SINGLE_STAGE_PLOT = new string[] { CruiseMethods.PNT, CruiseMethods.FIX };


        public static TreeStub_Plot CreateTree(string unitCode, int plotNumber, TallyPopulation_Plot population, string countOrMeasure, int treeCount = 1, int kpi = 0, bool stm = false)
        {
            return new TreeStub_Plot
            {
                CuttingUnitCode = unitCode,
                StratumCode = population.StratumCode,
                SampleGroupCode = population.SampleGroupCode,
                Species = population.Species,
                LiveDead = population.LiveDead,
                PlotNumber = plotNumber,
                CountOrMeasure = countOrMeasure,
                TreeCount = treeCount,
                KPI = kpi,
                STM = (stm) ? "Y" : "N",
                TreeID = Guid.NewGuid().ToString()
            };
        }

        public static async Task<TreeStub_Plot> TallyAsync(TallyPopulation_Plot pop,
            string unitCode, int plot,
            ISampleSelectorDataService sampleSelectorRepo,
            IDialogService dialogService)
        {
            if(SINGLE_STAGE_PLOT.Contains(pop.Method))
            {
                return CreateTree(unitCode, plot, pop, "M");
            }
            else if (pop.Is3P)//threeP sampling
            {
                int? kpi = await dialogService.AskKPIAsync(pop.MaxKPI, pop.MinKPI);
                if (kpi != null)
                {
                    return TallyThreeP(pop, kpi.Value, unitCode, plot, sampleSelectorRepo);
                }
                else
                { return null; }
            }
            else//non 3P sampling (STR)
            {
                return TallyStandard(pop, unitCode, plot, sampleSelectorRepo);
            }
        }

        //DataService (CreateNewTreeEntry)
        //SampleGroup (MinKPI/MaxKPI)
        public static TreeStub_Plot TallyThreeP(TallyPopulation_Plot pop,
            int kpi,
            string unitCode, int plot,
            ISampleSelectorDataService sampleSelectorRepo)
        {
            var sampler = sampleSelectorRepo.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();

            if (kpi == -1)  //user entered sure to measure
            {
                return CreateTree(unitCode, plot, pop, "M", stm: true);
            }
            else
            {
                ThreePItem item = (ThreePItem)((ThreePSelecter)sampler).NextItem();
                if (item != null && kpi > item.KPI)
                {
                    bool isInsuranceTree = sampler.IsSelectingITrees && sampler.InsuranceCounter.Next();

                    return CreateTree(unitCode, plot, pop, (isInsuranceTree) ? "I" : "M", kpi: kpi);
                }
                else
                {
                    return CreateTree(unitCode, plot, pop, "C", kpi: kpi);
                }
            }
        }

        //DataService (CreateNewTreeEntry)
        //
        public static TreeStub_Plot TallyStandard(TallyPopulation_Plot pop, string unitCode, int plot,
            ISampleSelectorDataService sampleSelectorRepo)
        {
            var sampler = sampleSelectorRepo.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();
            boolItem item = (boolItem)sampler.NextItem();

            //If we receive nothing from the sampler, we don't have a sample
            if (item != null)//&& (item.IsSelected || item.IsInsuranceItem))
            {
                return CreateTree(unitCode, plot, pop, (item.IsInsuranceItem) ? "I" : "M");
            }
            else
            {
                return CreateTree(unitCode, plot, pop, "C");
            }
        }
    }
}
