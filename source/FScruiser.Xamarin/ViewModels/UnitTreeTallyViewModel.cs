using FScruiser.Logic;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class UnitTreeTallyViewModel : ViewModelBase
    {
        public static readonly string STRATUM_FILTER_ALL = "All";

        private Dictionary<string, IEnumerable<TallyPopulation>> _tallies;
        private string _selectedStratumCode = STRATUM_FILTER_ALL;
        private ICommand _stratumSelectedCommand;
        private ICommand _tallyCommand;
        private IList<TallyEntry> _tallyFeed;
        private ICommand _editTreeCommand;

        public event EventHandler TallyEntryAdded;

        public IList<TallyEntry> TallyFeed
        {
            get { return _tallyFeed; }
            set { SetValue(ref _tallyFeed, value); }
        }

        public Dictionary<string, IEnumerable<TallyPopulation>> Tallies
        {
            get { return _tallies; }
            protected set
            {
                SetValue(ref _tallies, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
                RaisePropertyChanged(nameof(StrataFilterOptions));
            }
        }

        public IEnumerable<string> StrataFilterOptions
        {
            get
            {
                if (Tallies != null)
                { return Tallies.Keys.AsEnumerable().Append(STRATUM_FILTER_ALL).ToArray(); }
                else { return Enumerable.Empty<string>(); }
            }
        }

        public string SelectedStratumCode
        {
            get { return _selectedStratumCode; }
            set
            {
                SetValue(ref _selectedStratumCode, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
            }
        }

        public IEnumerable<TallyPopulation> TalliesFiltered
        {
            get
            {
                if (Tallies == null) { return Enumerable.Empty<TallyPopulation>(); }
                if (SelectedStratumCode == STRATUM_FILTER_ALL)
                {
                    return Tallies.Values.SelectMany(x => x).ToArray();
                }
                else
                {
                    return Tallies[SelectedStratumCode];
                }
            }
        }

        public string UnitCode { get; }

        public ICommand TallyCommand => _tallyCommand
            ?? (_tallyCommand = new Command<TallyPopulation>(async (x) => await this.TallyAsync(x)));

        public ICommand StratumSelectedCommand => _stratumSelectedCommand
            ?? (_stratumSelectedCommand = new Command<string>(x => SetStratumFilter(x)));

        public ICommand EditTreeCommand => _editTreeCommand
            ?? (_editTreeCommand = new Command<TallyEntry>(EditTree));

        public void EditTree(TallyEntry entry)
        {
            var task = DialogService.ShowEditTreeAsync(entry.Tree_GUID);
        }

        public ICuttingUnitDatastore Datastore { get; }
        public IDialogService DialogService { get; }
        public ISampleSelectorDataService SampleSelectorService { get; }
        public ITallySettingsDataService TallySettings { get; }
        public ISoundService SoundService { get; }

        public UnitTreeTallyViewModel(string unitCode, 
            ICuttingUnitDatastore datastore, 
            IDialogService dialogService, 
            ISampleSelectorDataService sampleSelectorDataService,
            ITallySettingsDataService tallySettings, 
            ISoundService soundService)
        {
            UnitCode = unitCode;
            Datastore = datastore;
            DialogService = dialogService;
            SampleSelectorService = sampleSelectorDataService;
            TallySettings = tallySettings;
            SoundService = soundService;
        }

        protected void RaiseTallyEntryAdded()
        {
            TallyEntryAdded?.Invoke(this, null);
        }

        public async Task InitAsync()
        {
            var datastore = Datastore;
            var unitCode = UnitCode;
            if (datastore != null)
            {
                //await dataService.RefreshDataAsync();

                var tallyLookup = datastore.GetTallyPopulationsByUnitCode(unitCode)
                    .GroupBy(x => x.StratumCode)
                    .ToDictionary(x => x.Key, x => (IEnumerable<TallyPopulation>)x.ToArray());
                Tallies = tallyLookup;

                TallyFeed = datastore.GetTallyEntriesByUnitCode(unitCode).Reverse().ToObservableCollection();
            }
        }

        private async Task TallyAsync(TallyPopulation pop)
        {
            var entry = await TreeBasedTallyLogic.TallyAsync(UnitCode, pop, Datastore, SampleSelectorService, DialogService);//TODO async
            Datastore.InsertTallyEntry(entry);

            TallyFeed.Add(entry);
            RaiseTallyEntryAdded();

            await HandleTally(pop, entry);
        }

        protected async Task HandleTally(TallyPopulation population,
            TallyEntry entry)
        {
            if (entry == null) { throw new ArgumentNullException(nameof(entry)); }

            population.TreeCount = population.TreeCount + entry.TreeCount;
            population.SumKPI = population.SumKPI + entry.KPI;

            SoundService.SignalTally();
            if (entry.HasTree)
            {
                if (entry.CountOrMeasure == "M")
                {
                    SoundService.SignalMeasureTree();
                }
                else if (entry.CountOrMeasure == "I")
                {
                    SoundService.SignalInsuranceTree();
                }

                if (TallySettings.EnableCruiserPopup)
                {
                    await DialogService.AskCruiserAsync(entry);
                }
                else
                {
                    var sampleType = (entry.CountOrMeasure == "M") ? "Measure Tree" :
                                (entry.CountOrMeasure == "I") ? "Insurance Tree" : String.Empty;
                    await DialogService.ShowMessageAsync("Tree #" + entry.TreeNumber.ToString(), sampleType);
                }

                //if (tree.CountOrMeasure == "M" && await AskEnterMeasureTreeDataAsync(tallySettings, dialogService))
                //{
                //    var task = dialogService.ShowEditTreeAsync(tree, dataService);//allow method to contiue from show edit tree we will allow tally history action to be added in the background
                //}
            }
        }

        public void Untally(TallyEntry tallyEntry)
        {
            Datastore.DeleteTally(tallyEntry);
            TallyFeed.Remove(tallyEntry);
        }

        public void SetStratumFilter(string code)
        {
            SelectedStratumCode = code ?? STRATUM_FILTER_ALL;
        }
    }
}