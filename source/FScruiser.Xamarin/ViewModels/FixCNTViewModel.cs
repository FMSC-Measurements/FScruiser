using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class FixCNTViewModel : ViewModelBase
    {
        private Command<FixCNTTallyBucket> _processTallyCommand;
        private bool _isUntallyEnabled;

        public IEnumerable<FixCntTallyPopulation> TallyPopulations { get; set; }

        public ICuttingUnitDatastore Datastore { get; protected set; }

        public ICommand ProcessTallyCommand => _processTallyCommand ?? (_processTallyCommand = new Command<FixCNTTallyBucket>(ProcessTally));

        public bool IsUntallyEnabled
        {
            get { return _isUntallyEnabled; }
            set { SetValue(ref _isUntallyEnabled, value); }
        }

        public FixCNTViewModel() { }

        public FixCNTViewModel(INavigationService navigationService, IDatastoreProvider datastoreProvider) : base(navigationService)
        {
            Datastore = datastoreProvider.Get<ICuttingUnitDatastore>();
        }

        public void Tally(FixCNTTallyBucket tallyBucket)
        {
            //var tree = tallyBucket.Tree;
            //tree.TreeCount++;
            //Datastore.UpdateTree(tree);
        }

        //public void Tally(string species, Double midValue)
        //{
        //    var tallyPopulation = TallyPopulations.Where(x => x.Species == species).First();

        //    var bucket = tallyPopulation.Buckets.Where(x => x.Value == midValue).Single();

        //    Tally(bucket);
        //}

        public void UnTally(FixCNTTallyBucket tallyBucket)
        {
            //var tree = tallyBucket.Tree;
            //var treeCount = tree.TreeCount;
            //if (treeCount > 0)
            //{
            //    tree.TreeCount = treeCount - 1;
            //    Datastore.UpdateTree(tree);
            //}
        }

        public void ProcessTally(FixCNTTallyBucket tallyBucket)
        {
            if(IsUntallyEnabled)
            {
                UnTally(tallyBucket);
            }
            else
            {
                Tally(tallyBucket);
            }
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
                var interval = tp.Min + tp.IntervalSize / 2;

                //foreach interval value try to read a tree
                do
                {
                    var tree = Datastore.GetFixCNTTallyTree(unit, plotNumber, stratumCode, tp.SampleGroupCode, tp.Species, tp.LiveDead, tp.FieldName, interval);
                    //if tree doesn't exist create it
                    if (tree == null)
                    {
                        tree = Datastore.CreateFixCNTTallyTree(unit, plotNumber, stratumCode, tp.SampleGroupCode, tp.Species, tp.LiveDead, tp.FieldName, interval);
                    }

                    buckets.Add(new FixCNTTallyBucket() { Value = interval, Tree = tree });

                    interval += tp.IntervalSize;
                } while (interval <= tp.Max);

                tp.Buckets = buckets;
            }

            TallyPopulations = tallyPopulations;
            RaisePropertyChanged(nameof(TallyPopulations));
        }
    }
}