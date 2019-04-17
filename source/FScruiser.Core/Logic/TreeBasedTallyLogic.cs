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
        static TallyAction CreateTally(string unitCode, TallyPopulation population, 
            bool isSample, bool isInsuranceSample = false, int treeCount = 1, int kpi = 0, int threePRandomeValue = 0, bool stm = false)
        {

            var tallyEntry = new TallyAction(unitCode, population)
            {
                IsSample = isSample, 
                IsInsuranceSample = isInsuranceSample,
                TreeCount = treeCount,
                KPI = kpi,
                ThreePRandomValue = threePRandomeValue,
                STM = stm,
            };

            return tallyEntry;
        }

        static TallyAction CreateTallyWithTree(string unitCode, TallyPopulation population, 
            bool isInsurance = false, int treeCount = 1, int kpi = 0, 
            int threePRandomeValue = 0, bool stm = false)
        {
            var tallyEntry = new TallyAction(unitCode, population)
            {
                CountOrMeasure = isInsurance ? TallyAction.CountOrMeasureValue.I : TallyAction.CountOrMeasureValue.M,
                TreeCount = treeCount,
                KPI = kpi,
                ThreePRandomValue = threePRandomeValue,
                STM = stm,
            };

            return tallyEntry;
        }

        public static async Task<TallyAction> TallyAsync(string unitCode,
            TallyPopulation pop,
            ICuttingUnitDatastore datastore,
            ISampleSelectorDataService samplerService,
            IDialogService dialogService)
        {

            if(pop.IsClickerTally)
            {
                var clickerTallyResult = await dialogService.AskTreeCount(pop.Frequency);
                if(clickerTallyResult != null && clickerTallyResult.TreeCount.HasValue)
                {
                    return CreateTally(unitCode, pop, isSample: true,
                    isInsuranceSample: false, treeCount: clickerTallyResult.TreeCount.Value);
                }
                else
                {
                    return null;
                }

                
            }

            var sampler = samplerService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();

            //if doing a manual tally create a tree and jump out
            if (sampler is ClickerSelecter clickerSelecter)
            {
                return CreateTally(unitCode, pop, isSample: true, 
                    isInsuranceSample: false, treeCount: clickerSelecter.Frequency);
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

        public static async Task<TallyAction> TallyS3P(string unitCode, TallyPopulation pop,
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
                        return CreateTally(unitCode, pop, isSample: true, 
                            isInsuranceSample: false, stm: true);
                    }
                    else
                    {
                        ThreePItem item3p = (ThreePItem)((ThreePSelecter)secondarySampler).NextItem();
                        if (item3p != null && kpi.Value > item3p.KPI)
                        {
                            bool isInsuranceTree = secondarySampler.IsSelectingITrees && secondarySampler.InsuranceCounter.Next();

                            return CreateTally(unitCode, pop, isSample: true, 
                                isInsuranceSample: isInsuranceTree, kpi: kpi.Value, threePRandomeValue:item3p.KPI);
                        }
                        else
                        {
                            return CreateTally(unitCode, pop, isSample: false, 
                                kpi: kpi.Value, threePRandomeValue: item3p.KPI);
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
                return CreateTally(unitCode, pop, isSample: false);
            }
        }

        //DataService (CreateNewTreeEntry)
        //SampleGroup (MinKPI/MaxKPI)
        public static TallyAction TallyThreeP(string unitCode, 
            TallyPopulation pop,
            int kpi,
            ISampleSelectorDataService samplerService)
        {
            var sampler = samplerService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();

            if (kpi == -1)  //user entered sure to measure
            {
                return CreateTally(unitCode, pop, isSample: true, isInsuranceSample: false, stm: true);
            }
            else
            {
                ThreePItem item = (ThreePItem)((ThreePSelecter)sampler).NextItem();
                if (item != null && kpi > item.KPI)
                {
                    bool isInsuranceTree = sampler.IsSelectingITrees && sampler.InsuranceCounter.Next();

                    return CreateTally(unitCode, pop, isSample: true, 
                        isInsuranceSample: isInsuranceTree, kpi: kpi, threePRandomeValue:item.KPI);
                }
                else
                {
                    return CreateTally(unitCode, pop, isSample: false, 
                        kpi: kpi, threePRandomeValue: item.KPI);
                }
            }
        }

        private static TallyAction TallyStandard(string unitCode, 
            TallyPopulation pop,
            ISampleSelectorDataService samplerService)
        {
            var sampler = samplerService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();
            boolItem item = (boolItem)sampler.NextItem();

            //If we receive nothing from the sampler, we don't have a sample
            if (item != null)//&& (item.IsSelected || item.IsInsuranceItem))
            {
                return CreateTally(unitCode, pop, isSample: true, isInsuranceSample: item.IsInsuranceItem);
            }
            else
            {
                return CreateTally(unitCode, pop, isSample: false);
            }
        }
    }
}