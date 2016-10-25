using Backpack;
using FMSC.Sampling;
using FScruiser.Models;
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
        public DatastoreRedux Datastore { get; set; }

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

        public StratumTalliesViewModel(DatastoreRedux datastore)
        {
            Datastore = datastore;
        }

        public override void Init(object initData)
        {
            Stratum = (UnitStratum)initData;

            TallyPopulations = Datastore.From<TallyPopulation>()
                .Where($"CuttingUnit_CN = {Stratum.CuttingUnit_CN} AND Stratum_CN = {Stratum.Stratum_CN}")
                .Read().ToList();

            foreach (var population in TallyPopulations)
            {
                population.Sampler = Datastore.From<Sampler>()
                    .Where($"Stratum_CN = {population.Stratum_CN} AND SampleGroup_CN = {population.SampleGroup_CN}")
                    .Read().FirstOrDefault();
            }

            if (Stratum.IsPlotStratum)
            {
                Plots = Datastore.From<PlotProxy>()
                    .Where($"CuttingUnit_CN = {Stratum.CuttingUnit_CN} AND Stratum_CN = {Stratum.Stratum_CN}")
                    .Read().ToList();
            }

            base.Init(initData);
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
            if (Stratum.IsPlotStratum)
            {
            }

            var sampler = tally.Sampler;

            if (sampler.CruiseMethod == "3P")
            {
                var tree = TallyThreeP(tally, sampler);
                if (tree != null)
                {
                    Datastore.Insert(tree, Backpack.SQL.OnConflictOption.Default);
                }
            }
            else if (sampler.CruiseMethod == "STR")
            {
                var tree = TallySTR(tally, sampler);
                if (tree != null)
                {
                    Datastore.Insert(tree, Backpack.SQL.OnConflictOption.Default);
                }
            }

            tally.TreeCount += 1;
        }

        Tree TallyThreeP(TallyPopulation tally, Sampler sampler)
        {
            var selector = sampler.Selector;

            int kpi;
            bool stm;
            if (AskKPI(sampler.MinKPI, sampler.MaxKPI, out kpi, out stm))
            {
                Tree tree = null;

                if (stm)
                {
                    tree = CreateTree(tally);
                    tree.STM = true;
                }
                else
                {
                    LogTreeEstimate(tally, kpi);
                    tally.SumKPI += kpi;

                    ThreePItem item = (ThreePItem)((ThreePSelecter)selector).NextItem();
                    if (item != null && kpi > item.KPI)
                    {
                        tree = CreateTree(tally);
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
                        tree = CreateTree(tally);
                        tree.CountOrMeasure = "C";
                    }
                }
                return tree;
            }
            return null;
        }

        Tree TallySTR(TallyPopulation tally, Sampler sampler)
        {
            var selector = sampler.Selector;

            var result = (boolItem)selector.NextItem();
            //If we receive nothing from the sampler, we don't have a sample
            if (result != null)
            {
                var tree = CreateTree(tally);

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
                var tree = CreateTree(tally);
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

        Tree CreateTree(TallyPopulation tally)
        {
            var tree = new Tree()
            {
                CuttingUnit_CN = Stratum.CuttingUnit_CN,
                Stratum_CN = Stratum.Stratum_CN,
                SampleGroup_CN = tally.SampleGroup_CN,
                TreeDefaultValue_CN = tally.TreeDefaultValue_CN
            };

            if (Stratum.IsPlotStratum)
            {
                tree.Plot_CN = CurrentPlot.Plot_CN;
                tree.TreeCount = 1;
            }

            return tree;
        }

        void LogTreeEstimate(TallyPopulation tally, int kpi)
        {
            var treeEstimate = new TreeEstimate
            {
                CountTree_CN = tally.CountTree_CN,
                KPI = kpi
            };

            Datastore.Insert(treeEstimate, Backpack.SQL.OnConflictOption.Default);
        }
    }
}