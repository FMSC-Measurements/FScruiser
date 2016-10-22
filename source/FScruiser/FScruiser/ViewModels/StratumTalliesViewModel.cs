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

        public IList<CountTree> Counts { get; set; }

        Dictionary<long, Sampler> _samplers { get; set; }

        public ICommand TallyCommand =>
            new Command<CountTree>((x) => Tally(x));

        public StratumTalliesViewModel(DatastoreRedux datastore)
        {
            Datastore = datastore;
        }

        public override void Init(object initData)
        {
            Stratum = (UnitStratum)initData;

            Counts = Datastore.From<CountTree>()
                .Where($"CuttingUnit_CN = {Stratum.CuttingUnit_CN} AND Stratum_CN = {Stratum.Stratum_CN}")
                .Read().ToList();

            var samplers = Datastore.From<Sampler>().Where($"Stratum_CN = {Stratum.Stratum_CN}").Read();
            foreach (var smplr in samplers)
            {
                if (!_samplers.ContainsKey(smplr.SampleGroup_CN))
                {
                    _samplers.Add(smplr.SampleGroup_CN, smplr);
                }
            }

            base.Init(initData);
        }

        protected void Tally(CountTree tally)
        {
            tally.TreeCount += 1;

            var sampler = _samplers[tally.SampleGroup_CN];
        }

        TreeEx TallyThreeP(CountTree tally, Sampler sampler)
        {
            var selector = sampler.Selector;

            int kpi;
            bool stm;
            if (AskKPI(sampler.MinKPI, sampler.MaxKPI, out kpi, out stm))
            {
                TreeEx tree = null;

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

        bool AskKPI(int min, int max, out int kpi, out bool stm)
        { throw new NotImplementedException(); }

        TreeEx CreateTree(CountTree tally)
        { throw new NotImplementedException(); }

        void LogTreeEstimate(int kpi)
        { throw new NotImplementedException(); }
    }
}