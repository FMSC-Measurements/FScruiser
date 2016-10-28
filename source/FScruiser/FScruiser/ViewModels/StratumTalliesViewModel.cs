using Backpack;
using FMSC.Sampling;
using FScruiser.Models;
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

        public UnitStratum Stratum { get; set; }

        IList<PlotProxy> _plots;

        public IList<PlotProxy> Plots
        {
            get { return _plots; }
            set { _plots = value; }
        }

        public PlotProxy CurrentPlot { get; set; }

        public IList<TallyPopulation> TallyPopulations { get; set; }

        public ICommand TallyCommand =>
            new Command<TallyPopulation>((x) => Tally(x));

        public ICommand ShowPlotInfoCommand =>
            new Command(() => ShowPlotInfo(), () => CurrentPlot != null);

        public ICommand AddPlotCommand =>
            new Command(() => ShowAddPlot());

        private void ShowAddPlot()
        {
            if (Stratum.IsPlotStratum)
            {
                CoreMethods.PushPageModel<PlotInfoViewModel>(null);
            }
        }

        private void ShowPlotInfo()
        {
            if (Stratum.IsPlotStratum && CurrentPlot != null)
            {
                CoreMethods.PushPageModel<PlotInfoViewModel>(CurrentPlot);
            }
        }

        public override void Init(object initData)
        {
            Stratum = (UnitStratum)initData;

            TallyPopulations = DataService.GetTallyPopulationByStratum(Stratum.StratumCode).ToList();

            foreach (var population in TallyPopulations)
            {
                population.Sampler = DataService.GetSamplerBySampleGroup(population.SampleGroupCode);
            }

            if (Stratum.IsPlotStratum)
            {
                Plots = DataService.GetPlotProxiesByStratum(Stratum.StratumCode).ToList();
            }

            base.Init(initData);
        }

        public override void ReverseInit(object returnedData)
        {
            base.ReverseInit(returnedData);
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

        protected void Tally(TallyPopulation tally)
        {
            //if is plot stratum, but no plot selected then bounce
            if (Stratum.IsPlotStratum && !EnsurePlotSelected()) { return; }

            if (CruiseMethods.THREE_P_METHODS.Contains(Stratum.CruiseMethod))
            {
                var tree = TallyThreeP(tally);
                if (tree != null)
                {
                    DataService.AddTree(tree);
                }
            }
            else if (CruiseMethods.STANDARD_SAMPLING_METHODS.Contains(Stratum.CruiseMethod))
            {
                var tree = TallyStandard(tally);
                if (tree != null)
                {
                    DataService.AddTree(tree);
                }
            }
            else if (Stratum.CruiseMethod == CruiseMethods.H_PCT)
            {
                var tree = TallyHpct(tally);
                if (tree != null)
                {
                    DataService.AddTree(tree);
                }
            }

            tally.TreeCount += 1;
        }

        Tree TallyHpct(TallyPopulation tally)
        {
            var tree = DataService.CreateNewTree(tally, CurrentPlot?.Plot_CN);
            tree.TreeCount = 1;
            tree.CountOrMeasure = "M";

            return tree;
        }

        Tree TallyThreeP(TallyPopulation tally)
        {
            var sampler = tally.Sampler;
            var selector = sampler.Selector;

            int kpi;
            bool stm;
            if (AskKPI(sampler.MinKPI, sampler.MaxKPI, out kpi, out stm))
            {
                Tree tree = null;

                if (stm)
                {
                    tree = DataService.CreateNewTree(tally, CurrentPlot?.Plot_CN);
                    tree.STM = true;
                }
                else
                {
                    DataService.LogTreeEstimate(kpi, tally);
                    tally.SumKPI += kpi;

                    ThreePItem item = (ThreePItem)((ThreePSelecter)selector).NextItem();
                    if (item != null && kpi > item.KPI)
                    {
                        tree = DataService.CreateNewTree(tally, CurrentPlot?.Plot_CN);
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
                    else if (Stratum.IsPlotStratum)
                    {
                        tree = DataService.CreateNewTree(tally, CurrentPlot?.Plot_CN);
                        tree.CountOrMeasure = "C";
                    }
                }
                return tree;
            }
            return null;
        }

        Tree TallyStandard(TallyPopulation tally)
        {
            var sampler = tally.Sampler;
            var selector = sampler.Selector;

            var result = (boolItem)selector.NextItem();
            //If we receive nothing from the sampler, we don't have a sample
            if (result != null)
            {
                var tree = DataService.CreateNewTree(tally, CurrentPlot?.Plot_CN);

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
            else if (Stratum.IsPlotStratum)
            {
                var tree = DataService.CreateNewTree(tally, CurrentPlot?.Plot_CN);
                tree.CountOrMeasure = "C";
                return tree;
            }
            else
            { return null; }
        }

        bool AskKPI(int min, int max, out int kpi, out bool stm)
        {
            var rand = new Random();
            kpi = rand.Next(min, max);
            stm = false;
            return true;
        }
    }
}