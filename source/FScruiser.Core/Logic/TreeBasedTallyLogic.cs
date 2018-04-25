using CruiseDAL.DataObjects;
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
    public class TreeBasedTallyLogic
    {
        public static void OnTally(TallyPopulation count,
            ICuttingUnitDataService dataService, ICollection<TallyFeedItem> tallyHistory,
            ITallySettingsDataService tallySettings,
            IDialogService dialogService, ISoundService soundService)
        {
            TallyFeedItem action = null;
            var sg = count.SampleGroup;

            //if doing a manual tally create a tree and jump out
            if (sg.SampleSelectorType == CruiseDAL.Schema.CruiseMethods.CLICKER_SAMPLER_TYPE)
            {
                try
                {
                    action = new TallyFeedItem()
                    {
                        Count = count
                    };
                    var newTree = dataService.CreateTree(count); //create measure tree
                    newTree.CountOrMeasure = "M";
                    newTree.TreeCount      = sg.SamplingFrequency;     //increment tree count on tally
                    action.Tree = newTree;
                    dataService.InsertTree(newTree);
                }
                catch (FMSC.ORM.SQLException e) //count save fail
                {
                    dialogService.ShowMessage("File error");
                }
            }
            else if (count.Is3P)//threeP sampling
            {
                action = TallyThreeP(count, sg.Sampler, sg, dataService, dialogService);
            }
            else//non 3P sampling (STR)
            {
                action = TallyStandard(count, sg.Sampler, dataService, dialogService);
            }

            //action may be null if cruising 3P and user doesn't enter a kpi
            if (action != null)
            {
                soundService.SignalTally();
                var tree = action.Tree;
                if (tree != null)
                {
                    if (tree.CountOrMeasure == "M")
                    {
                        soundService.SignalMeasureTree();
                    }
                    else if (tree.CountOrMeasure == "I")
                    {
                        soundService.SignalInsuranceTree();
                    }

                    if (tallySettings.EnableCruiserPopup)
                    {
                        dialogService.AskCruiser(tree);
                        dataService.UpdateTree(tree);
                    }
                    else
                    {
                        var sampleType = (tree.CountOrMeasure == "M") ? "Measure Tree" :
                                 (tree.CountOrMeasure == "I") ? "Insurance Tree" : String.Empty;
                        dialogService.ShowMessage("Tree #" + tree.TreeNumber.ToString(), sampleType);
                    }

                    if (tree.CountOrMeasure == "M" && AskEnterMeasureTreeData(tallySettings, dialogService))
                    {
                        dialogService.ShowEditTree(tree);
                    }
                }
                tallyHistory.Add(action);
            }
        }

        //DataService (CreateNewTreeEntry)
        //SampleGroup (MinKPI/MaxKPI)
        public static TallyFeedItem TallyThreeP(TallyPopulation count, SampleSelecter sampler, SampleGroup sg, ICuttingUnitDataService dataService, IDialogService dialogService)
        {
            var action = new TallyFeedItem()
            { Count = count };

            int kpi = 0;
            int? value = dialogService.AskKPI((int)sg.MinKPI, (int)sg.MaxKPI);
            if (value == null)
            {
                return null;
            }
            else
            {
                kpi = value.Value;
            }

            var originalCount  = count.TreeCount;
            var originalSumKPI = count.SumKPI;

            Tree tree = null;
            if (kpi == -1)  //user entered sure to measure
            {
                tree = dataService.CreateTree(count);
                tree.STM = "Y";
            }
            else
            {
                action.TreeEstimate = dataService.LogTreeEstimate(count, kpi);
                //action.KPI = kpi; //kpi on tally action is redundent since we have both tree estimate and tree
                count.SumKPI += kpi;

                ThreePItem item = (ThreePItem)((ThreePSelecter)sampler).NextItem();
                if (item != null && kpi > item.KPI)
                {
                    bool isInsuranceTree = sampler.IsSelectingITrees && sampler.InsuranceCounter.Next();

                    tree                = dataService.CreateTree(count);
                    tree.KPI            = kpi;
                    tree.CountOrMeasure = (isInsuranceTree) ? "I" : "M";
                }
            }

            try
            {
                if (tree != null)
                {
                    dataService.InsertTree(tree);
                    action.Tree = tree;
                }

                count.TreeCount++;
                dataService.UpdateCount(count);

                return action;
            }
            catch (FMSC.ORM.SQLException e) //count save fail
            {
                count.SumKPI    = originalSumKPI;
                count.TreeCount = originalCount;

                dialogService.ShowMessage("File error");

                return null;
            }
        }

        //DataService (CreateNewTreeEntry)
        //
        public static TallyFeedItem TallyStandard(TallyPopulation count, SampleSelecter sampleSelecter, ICuttingUnitDataService dataService, IDialogService dialogService)
        {
            var action = new TallyFeedItem()
            { Count = count };

            boolItem item = (boolItem)sampleSelecter.NextItem();

            var originalCount = count.TreeCount;

            Tree tree = null;
            //If we receive nothing from the sampler, we don't have a sample
            if (item != null)//&& (item.IsSelected || item.IsInsuranceItem))
            {
                tree = dataService.CreateTree(count);
                tree.CountOrMeasure = (item.IsInsuranceItem) ? "I" : "M";
            }

            try
            {
                if (tree != null)
                {
                    dataService.InsertTree(tree);
                    action.Tree = tree;
                }

                count.TreeCount++;
                dataService.UpdateCount(count);

                return action;
            }
            catch (FMSC.ORM.SQLException e) //count save fail
            {
                count.TreeCount = originalCount;

                dialogService.ShowMessage("File error", e.GetType().Name);

                return null;
            }
        }

        protected static bool AskEnterMeasureTreeData(ITallySettingsDataService appSettings, IDialogService dialogService)
        {
            if (!appSettings.EnableAskEnterTreeData) { return false; }

            return dialogService.AskYesNo("Would you like to enter tree data now?", "Sample", false);
        }

    }
}
