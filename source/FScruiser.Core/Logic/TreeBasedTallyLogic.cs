using CruiseDAL.Schema;
using FMSC.Sampling;
using FScruiser.Models;
using FScruiser.Services;
using System.Linq;
using System.Threading.Tasks;

namespace FScruiser.Logic
{
    public class TreeBasedTallyLogic
    {
        public static async Task<TallyEntry> TallyAsync(TallyPopulation pop,
            ICuttingUnitDataService dataService,
            IDialogService dialogService)
        {
            TallyEntry action = null;

            var sampler = dataService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();

            //if doing a manual tally create a tree and jump out
            if (sampler is ClickerSelecter clickerSelecter)
            {
                action = dataService.CreateTallyWithTree(pop, "M", clickerSelecter.Frequency);
            }
            else if (pop.Method == CruiseMethods.S3P)
            {
                action = await TallyS3P(pop, dataService, dialogService);
            }
            else if (pop.Is3P)//threeP sampling
            {
                int? kpi = await dialogService.AskKPIAsync(pop.MaxKPI, pop.MinKPI);
                if (kpi != null)
                {
                    action = TallyThreeP(pop, kpi.Value, dataService);
                }
            }
            else//non 3P sampling (STR)
            {
                action = TallyStandard(pop, dataService);
            }

            return action;
        }

        public static async Task<TallyEntry> TallyS3P(TallyPopulation pop,
            ICuttingUnitDataService dataService,
            IDialogService dialogService)
        {
            var samplers = dataService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode);
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
                        return dataService.CreateTallyWithTree(pop, "M", stm: true);
                    }
                    else
                    {
                        ThreePItem item3p = (ThreePItem)((ThreePSelecter)secondarySampler).NextItem();
                        if (item3p != null && kpi.Value > item3p.KPI)
                        {
                            bool isInsuranceTree = secondarySampler.IsSelectingITrees && secondarySampler.InsuranceCounter.Next();

                            return dataService.CreateTallyWithTree(pop, (isInsuranceTree) ? "I" : "M", kpi: kpi.Value);
                        }
                        else
                        {
                            return dataService.CreateTally(pop, kpi: kpi.Value);
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
                return dataService.CreateTally(pop);
            }
        }

        //DataService (CreateNewTreeEntry)
        //SampleGroup (MinKPI/MaxKPI)
        public static TallyEntry TallyThreeP(TallyPopulation pop,
            int kpi,
            ICuttingUnitDataService dataService)
        {
            var sampler = dataService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();

            if (kpi == -1)  //user entered sure to measure
            {
                return dataService.CreateTallyWithTree(pop, "M", stm: true);
            }
            else
            {
                ThreePItem item = (ThreePItem)((ThreePSelecter)sampler).NextItem();
                if (item != null && kpi > item.KPI)
                {
                    bool isInsuranceTree = sampler.IsSelectingITrees && sampler.InsuranceCounter.Next();

                    return dataService.CreateTallyWithTree(pop, (isInsuranceTree) ? "I" : "M", kpi: kpi);
                }
                else
                {
                    return dataService.CreateTally(pop, kpi: kpi);
                }
            }
        }

        //DataService (CreateNewTreeEntry)
        //
        public static TallyEntry TallyStandard(TallyPopulation pop,
            ICuttingUnitDataService dataService)
        {
            var sampler = dataService.GetSamplersBySampleGroupCode(pop.StratumCode, pop.SampleGroupCode).First();
            boolItem item = (boolItem)sampler.NextItem();

            //If we receive nothing from the sampler, we don't have a sample
            if (item != null)//&& (item.IsSelected || item.IsInsuranceItem))
            {
                return dataService.CreateTallyWithTree(pop, (item.IsInsuranceItem) ? "I" : "M");
            }
            else
            {
                return dataService.CreateTally(pop);
            }
        }
    }
}