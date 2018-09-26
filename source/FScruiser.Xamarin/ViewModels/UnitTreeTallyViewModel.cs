using FScruiser.Logic;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.Util;
using FScruiser.XF.Services;
using Prism.Navigation;
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

        public string UnitCode { get; set; }

        public ICommand TallyCommand => _tallyCommand
            ?? (_tallyCommand = new Command<TallyPopulation>(async (x) => await this.TallyAsync(x)));

        public ICommand StratumSelectedCommand => _stratumSelectedCommand
            ?? (_stratumSelectedCommand = new Command<string>(x => SetStratumFilter(x)));

        public ICommand EditTreeCommand => _editTreeCommand
            ?? (_editTreeCommand = new Command<string>(EditTree));

        public void EditTree(string tree_guid)
        {
            NavigationService.NavigateAsync("Tree", new NavigationParameters() { { "Tree_Guid", tree_guid } }, useModalNavigation: true);
        }

        public ICuttingUnitDatastore Datastore { get; }

        public IDialogService DialogService { get; }
        public ISampleSelectorDataService SampleSelectorService { get; }
        public ITallySettingsDataService TallySettings { get; }
        public ISoundService SoundService { get; }

        public UnitTreeTallyViewModel(INavigationService navigationService,
            ICuttingUnitDatastoreProvider datastoreProvider,
            IDialogService dialogService,
            ITallySettingsDataService tallySettings,
            ISoundService soundService) : base(navigationService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
            SampleSelectorService = datastoreProvider.SampleSelectorDataService;
            DialogService = dialogService;
            TallySettings = tallySettings;
            SoundService = soundService;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            MessagingCenter.Subscribe<object, string>(this, Messages.EDIT_TREE_CLICKED, (sender, tree_guid) => EditTree(tree_guid));
            MessagingCenter.Subscribe<object, TallyEntry>(this, Messages.UNTALLY_CLICKED, (sender, tallyEntry) => Untally(tallyEntry));
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            MessagingCenter.Unsubscribe<object>(this, Messages.EDIT_TREE_CLICKED);
            MessagingCenter.Unsubscribe<object>(this, Messages.UNTALLY_CLICKED);
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var unitCode = UnitCode = parameters.GetValue<string>("UnitCode");

            var datastore = Datastore;

            var tallyLookup = datastore.GetTallyPopulationsByUnitCode(UnitCode)
                .GroupBy(x => x.StratumCode)
                .ToDictionary(x => x.Key, x => (IEnumerable<TallyPopulation>)x.ToArray());
            Tallies = tallyLookup;

            TallyFeed = datastore.GetTallyEntriesByUnitCode(UnitCode).Reverse().ToObservableCollection();
        }

        protected void RaiseTallyEntryAdded()
        {
            TallyEntryAdded?.Invoke(this, null);
        }

        public async Task TallyAsync(TallyPopulation pop)
        {
            var entry = await TreeBasedTallyLogic.TallyAsync(UnitCode, pop, Datastore, SampleSelectorService, DialogService);//TODO async

            if (entry == null) { return; }
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
                    var cruiser = await DialogService.AskCruiserAsync();
                    if (cruiser != null)
                    {
                        entry.Initials = cruiser;
                    }
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