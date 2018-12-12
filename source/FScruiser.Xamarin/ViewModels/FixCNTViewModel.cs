using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FScruiser.XF.ViewModels
{
    public class FixCNTViewModel : ViewModelBase
    {
        public IEnumerable<FixCntTallyPopulation> TallyPopulations { get; set; }

        public ICuttingUnitDatastore Datastore { get; set; }

        public FixCNTViewModel(INavigationService navigationService, ICuttingUnitDatastoreProvider datastoreProvider) : base(navigationService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
        }

        public void Tally(string species, Double midValue)
        {
            var tallyPopulation = TallyPopulations.Where(x => x.Species == species).First();

            var bucket = tallyPopulation.Buckets.Where(x => x.Value == midValue).Single();

            var tree = bucket.Tree;
            tree.TreeCount++;
            Datastore.UpdateTree(tree);
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var unit = parameters.GetValue<string>(NavParams.UNIT);
            var stratumCode = parameters.GetValue<string>(NavParams.STRATUM);
            var plotNumber = parameters.GetValue<int>(NavParams.PLOT_NUMBER);

            //read fixcount tally populations
            var tallyPopulations = Datastore.GetFixCNTTallyPopulations(stratumCode).ToArray();

            //foreach tally population calculate and itterate through interval values
            foreach (var tp in tallyPopulations)
            {
                var buckets = new List<FixCNTTallyBucket>();
                var interval = tp.IntervalMin + tp.IntervalSize / 2;

                //foreach interval value try to read a tree
                do
                {
                    var tree = Datastore.GetFixCNTTallyTree(unit, plotNumber, stratumCode, tp.SGCode, tp.TreeDefaultValue_CN, tp.FieldName, interval);
                    //if tree doesn't exist create it
                    if (tree == null)
                    {
                        tree = Datastore.CreateFixCNTTallyTree(unit, plotNumber, stratumCode, tp.SGCode, tp.TreeDefaultValue_CN, tp.FieldName, interval);
                    }

                    buckets.Add(new FixCNTTallyBucket() { Value = interval, Tree = tree });

                    interval += tp.IntervalSize;
                } while (interval <= tp.IntervalMax);

                tp.Buckets = buckets;
            }

            TallyPopulations = tallyPopulations;
        }
    }
}