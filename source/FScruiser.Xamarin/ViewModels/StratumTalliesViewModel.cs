﻿using FMSC.Sampling;
using FScruiser.Models;
using FScruiser.Pages;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.ViewModels
{
    public class StratumTalliesViewModel : FreshMvvm.FreshBasePageModel
    {
        public ICuttingUnitDataService DataService { get; set; }

        public StratumTalliesViewModel(ICuttingUnitDataService dataService)
        {
            DataService = dataService;
        }

        public Plot CurrentPlot { get; set; }
        public UnitStratum UnitStratum { get; set; }
        public IList<TallyPopulation> TallyPopulations { get; set; }

        public event EventHandler PlotsChanged;

        #region Commands

        public ICommand TallyCommand =>
            new Command<TallyPopulation>((x) => Tally(x));

        public ICommand ShowPlotInfoCommand =>
            new Command(() => ShowPlotInfo(), () => CurrentPlot != null);

        public ICommand ShowTreesCommand =>
            new Command(() => ShowTrees());

        public ICommand AddPlotCommand =>
            new Command(() => ShowAddPlot());

        public object TreeListData { get; private set; }

        public void ShowTrees()
        {
            var filter = new TreeListFilter
            {
                Stratum = UnitStratum.Stratum,
                Plot = CurrentPlot
            };

            CoreMethods.PushPageModel<TreeMasterDetailViewModel>(filter, true);
        }

        private async void ShowAddPlot()
        {
            if (UnitStratum.Stratum.IsPlotStratum)
            {
                var plot = DataService.CreateNewPlot(UnitStratum.Stratum.Code);
                await CoreMethods.PushPageModel<PlotEditViewModel>(plot);
                CurrentPlot = plot;
                OnPlotsChanged();
            }
        }

        private void ShowPlotInfo()
        {
            if (UnitStratum.Stratum.IsPlotStratum && CurrentPlot != null)
            {
                CoreMethods.PushPageModel<PlotEditViewModel>(CurrentPlot);
            }
        }

        #endregion Commands

        public override void Init(object initData)
        {
            UnitStratum = (UnitStratum)initData;

            TallyPopulations = DataService.GetTallyPopulationByStratum(UnitStratum.Stratum.Code).ToList();

            if (UnitStratum.Stratum.IsPlotStratum)
            {
                CurrentPlot = UnitStratum.Plots.LastOrDefault();
            }

            base.Init(initData);
        }

        protected void OnPlotsChanged()
        {
            PlotsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);
            DataService.SaveChangesAsync();
        }

        bool AskKPI(int min, int max, out int kpi, out bool stm)
        {
            var rand = new Random();
            kpi = rand.Next(min, max);
            stm = false;
            return true;
        }

        public void Tally(TallyPopulation tally)
        {
            //if is plot stratum, but no plot selected then bounce
            if (UnitStratum.Stratum.IsPlotStratum && !EnsurePlotSelected()) { return; }

            if (CruiseMethods.THREE_P_METHODS.Contains(UnitStratum.Stratum.Method))
            {
                int kpi;
                bool stm;
                if (AskKPI(tally.Sampler.MinKPI, tally.Sampler.MaxKPI, out kpi, out stm))
                {
                    if (stm)
                    { TallySTM(tally); }
                    else
                    {
                        var tree = TallyThreeP(tally, kpi);
                        if (tree != null)
                        {
                            DataService.AddTree(tree);
                            CoreMethods.PushPageModel<TreeEditViewModel>(tree);
                        }
                    }
                }
            }
            else if (CruiseMethods.STANDARD_SAMPLING_METHODS.Contains(UnitStratum.Stratum.Method))
            {
                var tree = TallyStandard(tally);
                if (tree != null)
                {
                    DataService.AddTree(tree);
                    CoreMethods.PushPageModel<TreeEditViewModel>(tree);
                }
            }
            else if (UnitStratum.Stratum.Method == CruiseMethods.H_PCT)
            {
                var tree = TallyHpct(tally);
                if (tree != null)
                {
                    DataService.AddTree(tree);
                    CoreMethods.PushPageModel<TreeEditViewModel>(tree);
                }
            }

            tally.TreeCount += 1;
        }

        Tree TallyHpct(TallyPopulation tally)
        {
            var tree = DataService.CreateNewTree(tally, CurrentPlot);
            tree.TreeCount = 1;
            tree.CountOrMeasure = "M";

            return tree;
        }

        Tree TallySTM(TallyPopulation tally)
        {
            var tree = DataService.CreateNewTree(tally, CurrentPlot);
            tree.STM = true;

            return tree;
        }

        Tree TallyThreeP(TallyPopulation tally, int kpi)
        {
            var sampler = tally.Sampler;
            var selector = sampler.Selector;
            Tree tree = null;

            DataService.LogTreeEstimate(kpi, tally);
            tally.SumKPI += kpi;

            ThreePItem item = (ThreePItem)selector.NextItem();
            if (item != null && kpi > item.KPI)
            {
                tree = DataService.CreateNewTree(tally, CurrentPlot);
                tree.KPI = kpi;

                if (selector.IsSelectingITrees && selector.InsuranceCounter.Next())
                {
                    tree.CountOrMeasure = "I";
                }
                else
                {
                    tree.CountOrMeasure = "M";
                }
            }
            else if (UnitStratum.Stratum.IsPlotStratum)
            {
                tree = DataService.CreateNewTree(tally, CurrentPlot);
                tree.CountOrMeasure = "C";
            }

            return tree;
        }

        Tree TallyStandard(TallyPopulation tally)
        {
            var sampler = tally.Sampler;

            var result = (boolItem)sampler.NextItem();
            //If we receive nothing from the sampler, we don't have a sample
            if (result != null)
            {
                var tree = DataService.CreateNewTree(tally, CurrentPlot);

                if (result.IsInsuranceItem)
                {
                    tree.CountOrMeasure = "I";
                }
                else
                {
                    tree.CountOrMeasure = "M";
                }

                return tree;
            }
            else if (UnitStratum.Stratum.IsPlotStratum)
            {
                var tree = DataService.CreateNewTree(tally, CurrentPlot);
                tree.CountOrMeasure = "C";
                return tree;
            }
            else
            { return null; }
        }

        bool EnsurePlotSelected()
        {
            if (CurrentPlot != null) { return true; }
            else
            {
                CoreMethods.DisplayAlert("Alert", "No Plot Selected", "OK");
                return false;
            }
        }
    }
}