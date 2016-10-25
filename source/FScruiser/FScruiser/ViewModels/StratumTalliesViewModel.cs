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

        public IList<TallyPopulation> TallyPopulations { get; set; }

        public ICommand TallyCommand =>
            new Command<TallyPopulation>((x) => Tally(x));

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

            base.Init(initData);
        }

        protected void Tally(TallyPopulation tally)
        {
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
                    LogTreeEstimate(kpi);
                    tally.SumKPI += kpi;

                    ThreePItem item = (ThreePItem)((ThreePSelecter)selector).NextItem();
                    if (item != null && kpi > item.KPI)
                    {
                        tree = CreateTree(tally);
                        tree.KPI = kpi;

                        if (selector.IsSelectingITrees && selector.InsuranceCounter.Next())
                        {
                            tree.CountMeasure = "I";
                        }
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
                    tree.CountMeasure = "I";
                }
                else
                {
                    tree.CountMeasure = "M";
                }

                return tree;
            }
            else
            { return null; }
        }

        bool AskKPI(int min, int max, out int kpi, out bool stm)
        { throw new NotImplementedException(); }

        Tree CreateTree(TallyPopulation tally)
        {
            var tree = new Tree()
            {
                CuttingUnit_CN = Stratum.CuttingUnit_CN,
                Stratum_CN = Stratum.Stratum_CN,
                SampleGroup_CN = tally.SampleGroup_CN,
                TreeDefaultValue_CN = tally.TreeDefaultValue_CN
            };

            return tree;
        }

        void LogTreeEstimate(int kpi)
        { throw new NotImplementedException(); }
    }
}