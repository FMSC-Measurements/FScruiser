using CruiseDAL.Schema;
using FMSC.Sampling;
using FScruiser.Models;
using FScruiser.Services;
using System.Threading.Tasks;

namespace FScruiser.Logic
{
    public class TreeBasedTallyLogic
    {
        public static async Task<TallyEntry> TallyAsync(TallyPopulation count,
            ICuttingUnitDataService dataService,
            IDialogService dialogService)
        {
            TallyEntry action = null;
            var sg = count.SampleGroup;

            //if doing a manual tally create a tree and jump out
            if (sg.SampleSelectorType == CruiseDAL.Schema.CruiseMethods.CLICKER_SAMPLER_TYPE)
            {
                action = new TallyEntry(count);

                var newTree = dataService.CreateTree(count); //create measure tree
                newTree.CountOrMeasure = "M";
                newTree.TreeCount = (int)sg.SamplingFrequency;     //increment tree count on tally
                action.SetTree(newTree);
            }
            else if (count.Method == CruiseMethods.S3P)
            {
                action = await TallyS3P(count, dataService, dialogService);
            }
            else if (count.Is3P)//threeP sampling
            {
                int? kpi = await dialogService.AskKPIAsync((int)sg.MaxKPI, (int)sg.MinKPI);
                if (kpi != null)
                {
                    action = TallyThreeP(count, kpi.Value, dataService);
                }
            }
            else//non 3P sampling (STR)
            {
                action = TallyStandard(count, dataService);
            }

            return action;
        }

        public static async Task<TallyEntry> TallyS3P(TallyPopulation pop,
            ICuttingUnitDataService dataService,
            IDialogService dialogService)
        {
            var sg = pop.SampleGroup;
            var sampler = sg.Sampler;

            var tallyEntry = new TallyEntry(pop)
            {
                TreeCount = 1
            };

            boolItem item = (boolItem)sampler.NextItem();

            Tree tree = null;
            //If we receive nothing from the sampler, we don't have a sample
            if (item != null)//&& (item.IsSelected || item.IsInsuranceItem))
            {
                var secondarySampler = sg.SecondarySampler;

                int? kpi = await dialogService.AskKPIAsync((int)sg.MaxKPI, (int)sg.MinKPI);
                if (kpi != null)
                {
                    if (kpi == -1)  //user entered sure to measure
                    {
                        tree = dataService.CreateTree(pop);
                        tree.STM = "Y";
                        tallyEntry.IsSTM = true;
                        tallyEntry.SetTree(tree);
                    }
                    else
                    {
                        tallyEntry.KPI = kpi.Value;

                        ThreePItem item3p = (ThreePItem)((ThreePSelecter)sampler).NextItem();
                        if (item3p != null && kpi.Value > item3p.KPI)
                        {
                            bool isInsuranceTree = sampler.IsSelectingITrees && sampler.InsuranceCounter.Next();

                            tree = dataService.CreateTree(pop);
                            tree.KPI = kpi.Value;
                            tree.CountOrMeasure = (isInsuranceTree) ? "I" : "M";
                            tallyEntry.SetTree(tree);
                        }
                    }
                }
                else
                {
                    return null;
                }
            }

            return tallyEntry;
        }

        //DataService (CreateNewTreeEntry)
        //SampleGroup (MinKPI/MaxKPI)
        public static TallyEntry TallyThreeP(TallyPopulation pop,
            int kpi,
            ICuttingUnitDataService dataService)
        {
            var sampler = pop.SampleGroup.Sampler;

            var tallyEntry = new TallyEntry(pop)
            {
                TreeCount = 1
            };

            Tree tree = null;
            if (kpi == -1)  //user entered sure to measure
            {
                tree = dataService.CreateTree(pop);
                tree.STM = "Y";
                tree.CountOrMeasure = "M";
                tallyEntry.IsSTM = true;
                tallyEntry.SetTree(tree);
            }
            else
            {
                tallyEntry.KPI = kpi;

                ThreePItem item = (ThreePItem)((ThreePSelecter)sampler).NextItem();
                if (item != null && kpi > item.KPI)
                {
                    bool isInsuranceTree = sampler.IsSelectingITrees && sampler.InsuranceCounter.Next();

                    tree = dataService.CreateTree(pop);
                    tree.KPI = kpi;
                    tree.CountOrMeasure = (isInsuranceTree) ? "I" : "M";
                    tallyEntry.SetTree(tree);
                }
            }

            return tallyEntry;
        }

        //DataService (CreateNewTreeEntry)
        //
        public static TallyEntry TallyStandard(TallyPopulation pop,
            ICuttingUnitDataService dataService)
        {
            var sg = pop.SampleGroup;
            var sampler = sg.Sampler;

            boolItem item = (boolItem)sampler.NextItem();

            var tallyEntry = new TallyEntry(pop)
            {
                TreeCount = 1
            };

            Tree tree = null;
            //If we receive nothing from the sampler, we don't have a sample
            if (item != null)//&& (item.IsSelected || item.IsInsuranceItem))
            {
                tree = dataService.CreateTree(pop);
                tree.CountOrMeasure = (item.IsInsuranceItem) ? "I" : "M";
                tallyEntry.SetTree(tree);
            }

            return tallyEntry;
        }
    }
}