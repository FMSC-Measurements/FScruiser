using CruiseDAL.Schema;
using FMSC.Sampling;
using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.Logic
{
    public class TreeBasedTallyLogic
    {
        static TallyEntry CreateTally(string unitCode, TallyPopulation population, int treeCount = 1, int kpi = 0, int threePRandomeValue = 0, bool stm = false)
        {
            var tallyEntry = new TallyEntry(unitCode, population)
            {
                CountOrMeasure = "C",
                TreeCount = treeCount,
                KPI = kpi,
                ThreePRandomValue = threePRandomeValue,
                IsSTM = stm,
                EntryType = Constants.TallyLedger_EntryType.Tally
            };

            return tallyEntry;
        }

        static TallyEntry CreateTallyWithTree(string unitCode, TallyPopulation population, string countOrMeasure, int treeCount = 1, int kpi = 0, int threePRandomeValue = 0, bool stm = false)
        {
            var tallyEntry = new TallyEntry(unitCode, population)
            {
                CountOrMeasure = countOrMeasure,
                TreeCount = treeCount,
                KPI = kpi,
                ThreePRandomValue = threePRandomeValue,
                IsSTM = stm,
                Tree_GUID = Guid.NewGuid().ToString(),
                EntryType = Constants.TallyLedger_EntryType.Tally
            };

            return tallyEntry;
        }

        public static async Task<TallyEntry> TallyAsync(string unitCode,
            TallyPopulation pop,
            ICuttingUnitDatastore datastore,
            ISampleSelectorDataService samplerService,
            IDialogService dialogService)
        {

            if(pop.IsClickerTally)
            {
                return CreateTallyWithTree(unitCode, pop, "M", pop.Frequency);
            }

            var sampler = samplerService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();

            //if doing a manual tally create a tree and jump out
            if (sampler is ClickerSelecter clickerSelecter)
            {
                return CreateTallyWithTree(unitCode, pop, "M", clickerSelecter.Frequency);
            }
            else if (pop.Method == CruiseMethods.S3P)
            {
                return await TallyS3P(unitCode, pop, samplerService, dialogService);
            }
            else if (pop.Is3P)//threeP sampling
            {
                int? kpi = await dialogService.AskKPIAsync(pop.MaxKPI, pop.MinKPI);
                if (kpi != null)
                {
                    return TallyThreeP(unitCode, pop, kpi.Value, samplerService);
                }
                else { return null; }//user didn't enter a kpi, so don't create a tally entry
            }
            else//non 3P sampling (STR)
            {
                return TallyStandard(unitCode, pop, samplerService);
            }
        }

        public static async Task<TallyEntry> TallyS3P(string unitCode, TallyPopulation pop,
            ISampleSelectorDataService samplerService,
            IDialogService dialogService)
        {
            var samplers = samplerService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode);
            var firstSamplet = samplers.ElementAt(0);

            boolItem item = (boolItem)firstSamplet.NextItem();

            //If we receive nothing from the sampler, we don't have a sample
            if (item != null)//&& (item.IsSelected || item.IsInsuranceItem))
            {
                var secondarySampler = samplers.ElementAt(1);

                int? kpi = await dialogService.AskKPIAsync(pop.MaxKPI, pop.MinKPI);
                if (kpi != null)
                {
                    if (kpi == -1)  //user entered sure to measure
                    {
                        return CreateTallyWithTree(unitCode, pop, "M", stm: true);
                    }
                    else
                    {
                        ThreePItem item3p = (ThreePItem)((ThreePSelecter)secondarySampler).NextItem();
                        if (item3p != null && kpi.Value > item3p.KPI)
                        {
                            bool isInsuranceTree = secondarySampler.IsSelectingITrees && secondarySampler.InsuranceCounter.Next();

                            return CreateTallyWithTree(unitCode, pop, (isInsuranceTree) ? "I" : "M", kpi: kpi.Value, threePRandomeValue:item3p.KPI);
                        }
                        else
                        {
                            return CreateTally(unitCode, pop, kpi: kpi.Value, threePRandomeValue: item3p.KPI);
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return CreateTally(unitCode, pop);
            }
        }

        //DataService (CreateNewTreeEntry)
        //SampleGroup (MinKPI/MaxKPI)
        public static TallyEntry TallyThreeP(string unitCode, 
            TallyPopulation pop,
            int kpi,
            ISampleSelectorDataService samplerService)
        {
            var sampler = samplerService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();

            if (kpi == -1)  //user entered sure to measure
            {
                return CreateTallyWithTree(unitCode, pop, "M", stm: true);
            }
            else
            {
                ThreePItem item = (ThreePItem)((ThreePSelecter)sampler).NextItem();
                if (item != null && kpi > item.KPI)
                {
                    bool isInsuranceTree = sampler.IsSelectingITrees && sampler.InsuranceCounter.Next();

                    return CreateTallyWithTree(unitCode, pop, (isInsuranceTree) ? "I" : "M", kpi: kpi, threePRandomeValue:item.KPI);
                }
                else
                {
                    return CreateTally(unitCode, pop, kpi: kpi, threePRandomeValue: item.KPI);
                }
            }
        }

        //DataService (CreateNewTreeEntry)
        //
        public static TallyEntry TallyStandard(string unitCode, 
            TallyPopulation pop,
            ISampleSelectorDataService samplerService)
        {
            var sampler = samplerService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();
            boolItem item = (boolItem)sampler.NextItem();

            //If we receive nothing from the sampler, we don't have a sample
            if (item != null)//&& (item.IsSelected || item.IsInsuranceItem))
            {
                return CreateTallyWithTree(unitCode, pop, (item.IsInsuranceItem) ? "I" : "M");
            }
            else
            {
                return CreateTally(unitCode, pop);
            }
        }
    }
}