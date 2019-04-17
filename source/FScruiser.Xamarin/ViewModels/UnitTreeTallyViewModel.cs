using CruiseDAL.Schema;
using FScruiser.Logic;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.Util;
using FScruiser.XF.Constants;
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

        private IList<TallyEntry> _tallyFeed;

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

        #region Commands

        private ICommand _editTreeCommand;
        private ICommand _showTallyMenuCommand;
        private ICommand _stratumSelectedCommand;
        private ICommand _tallyCommand;

        public ICommand ShowTallyMenuCommand => _showTallyMenuCommand
            ?? (_showTallyMenuCommand = new Command<TallyPopulation>(ShowTallyMenu));

        public ICommand TallyCommand => _tallyCommand
            ?? (_tallyCommand = new Command<TallyPopulation>(async (x) => await this.TallyAsync(x)));

        public ICommand StratumSelectedCommand => _stratumSelectedCommand
            ?? (_stratumSelectedCommand = new Command<string>(x => SetStratumFilter(x)));

        public ICommand EditTreeCommand => _editTreeCommand
            ?? (_editTreeCommand = new Command<string>(EditTree));

        #endregion Commands

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

            MessagingCenter.Subscribe<object, string>(this, Messages.EDIT_TREE_CLICKED, (sender, treeID) => EditTree(treeID));
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

       

        private void ShowTallyMenu(TallyPopulation obj)
        {
            NavigationService.NavigateAsync($"TreeCountEdit?{NavParams.UNIT}={UnitCode}&{NavParams.STRATUM}={obj.StratumCode}&{NavParams.SAMPLE_GROUP}={obj.SampleGroupCode}&{NavParams.SPECIES}={obj.Species}&{NavParams.LIVE_DEAD}={obj.LiveDead}",
                useModalNavigation: true);
        }

        public void EditTree(string treeID)
        {
            NavigationService.NavigateAsync("Tree", new NavigationParameters() { { NavParams.TreeID, treeID } }, useModalNavigation: true);
        }

        public async Task TallyAsync(TallyPopulation pop)
        {
            // perform logic to determin if tally is a sample
            var action = await TreeBasedTallyLogic.TallyAsync(UnitCode, pop, Datastore, SampleSelectorService, DialogService);//TODO async

            // record action to the database,
            // database will assign tree a tree number if there is a tree
            // action might be null if user dosn't enter kpi or tree count for clicker entry
            if (action == null) { return; }
            var entry = Datastore.InsertTallyAction(action);

            // trigger updates due to tally
            await HandleTally(pop, action, entry);
        }

        protected async Task HandleTally(TallyPopulation population,
            TallyAction action, TallyEntry entry)
        {
            if (entry == null) { throw new ArgumentNullException(nameof(entry)); }
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            TallyFeed.Add(entry);
            RaiseTallyEntryAdded();

            population.TreeCount = population.TreeCount + action.TreeCount;
            population.SumKPI = population.SumKPI + action.KPI;

            SoundService.SignalTally();
            if (action.IsSample)
            {
                var method = population.Method;

                if (action.IsInsuranceSample)
                {
                    SoundService.SignalInsuranceTree();
                }
                else
                {
                    SoundService.SignalMeasureTree();
                }

                if (TallySettings.EnableCruiserPopup)
                {
                    var cruiser = await DialogService.AskCruiserAsync();
                    if (cruiser != null)
                    {
                        Datastore.UpdateTreeInitials(entry.TreeID, cruiser);
                    }
                }
                else if(method != CruiseMethods.H_PCT)
                {
                    var sampleType = (action.IsInsuranceSample) ? "Insurance Tree" : "Measure Tree";
                    await DialogService.ShowMessageAsync("Tree #" + entry.TreeNumber.ToString(), sampleType);
                }

                //if (tree.CountOrMeasure == "M" && await AskEnterMeasureTreeDataAsync(tallySettings, dialogService))
                //{
                //    var task = dialogService.ShowEditTreeAsync(tree, dataService);//allow method to contiue from show edit tree we will allow tally history action to be added in the background
                //}
            }
        }

        protected void RaiseTallyEntryAdded()
        {
            TallyEntryAdded?.Invoke(this, null);
        }

        public void Untally(TallyEntry entry)
        {
            Datastore.DeleteTallyEntry(entry.TallyLedgerID);
            TallyFeed.Remove(entry);
        }

        public void SetStratumFilter(string code)
        {
            SelectedStratumCode = code ?? STRATUM_FILTER_ALL;
        }
    }
}