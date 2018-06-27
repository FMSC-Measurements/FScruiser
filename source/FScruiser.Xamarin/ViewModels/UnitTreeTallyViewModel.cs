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

        public ICommand TallyCommand => _tallyCommand
            ?? (_tallyCommand = new Command<TallyPopulation>(async (x) => await this.TallyAsync(x)));

        public ICommand StratumSelectedCommand => _stratumSelectedCommand
            ?? (_stratumSelectedCommand = new Command<string>(x => SetStratumFilter(x)));

        public ICommand EditTreeCommand => _editTreeCommand
            ?? (_editTreeCommand = new Command<TallyEntry>(EditTree));

        public void EditTree(TallyEntry entry)
        {
            var dialogService = ServiceService.DialogService;

            var task = dialogService.ShowEditTreeAsync(entry.Tree_GUID);
        }

        public ICuttingUnitDataService DataService => ServiceService.CuttingUnitDataService;

        public UnitTreeTallyViewModel()
        {
            //MessagingCenter.Subscribe<object>(this, Messages.CUTTING_UNIT_SELECTED, async (x) =>
            //{
            //    await InitAsync();
            //});
        }

        protected void RaiseTallyEntryAdded()
        {
            TallyEntryAdded?.Invoke(this, null);
        }

        public async Task InitAsync()
        {
            var dataService = DataService;
            if (DataService != null)
            {
                //await dataService.RefreshDataAsync();

                var tallyLookup = dataService.GetTallyPopulations()
                    .GroupBy(x => x.StratumCode)
                    .ToDictionary(x => x.Key, x => (IEnumerable<TallyPopulation>)x.ToArray());
                Tallies = tallyLookup;

                TallyFeed = dataService.GetTallyEntries().Reverse().ToObservableCollection();
            }
        }

        private async Task TallyAsync(TallyPopulation pop)
        {
            ITallySettingsDataService tallySettings = ServiceService.TallySettingsDataService;
            IDialogService dialogService = ServiceService.DialogService;
            ISoundService soundService = ServiceService.SoundService;
            ICuttingUnitDataService dataService = DataService;

            var entry = await TreeBasedTallyLogic.TallyAsync(pop, dataService, dialogService);//TODO async

            TallyFeed.Add(entry);
            RaiseTallyEntryAdded();

            await HandleTally(pop, entry, dataService, soundService, dialogService, tallySettings);
        }

        public static async Task HandleTally(TallyPopulation population,
            TallyEntry entry,
            ICuttingUnitDataService dataService,
            ISoundService soundService,
            IDialogService dialogService,
            ITallySettingsDataService tallySettings)
        {
            if (entry == null) { throw new ArgumentNullException(nameof(entry)); }

            population.TreeCount = population.TreeCount + entry.TreeCount;
            population.SumKPI = population.SumKPI + entry.KPI;

            soundService.SignalTally();
            if (entry.HasTree)
            {
                if (entry.CountOrMeasure == "M")
                {
                    soundService.SignalMeasureTree();
                }
                else if (entry.CountOrMeasure == "I")
                {
                    soundService.SignalInsuranceTree();
                }

                if (tallySettings.EnableCruiserPopup)
                {
                    await dialogService.AskCruiserAsync(entry);
                }
                else
                {
                    var sampleType = (entry.CountOrMeasure == "M") ? "Measure Tree" :
                                (entry.CountOrMeasure == "I") ? "Insurance Tree" : String.Empty;
                    await dialogService.ShowMessageAsync("Tree #" + entry.TreeNumber.ToString(), sampleType);
                }

                //if (tree.CountOrMeasure == "M" && await AskEnterMeasureTreeDataAsync(tallySettings, dialogService))
                //{
                //    var task = dialogService.ShowEditTreeAsync(tree, dataService);//allow method to contiue from show edit tree we will allow tally history action to be added in the background
                //}
            }
        }

        public void Untally(TallyEntry tallyEntry)
        {
            DataService.DeleteTally(tallyEntry);
            TallyFeed.Remove(tallyEntry);
        }

        //protected static async Task<bool> AskEnterMeasureTreeDataAsync(ITallySettingsDataService appSettings, IDialogService dialogService)
        //{
        //    if (!appSettings.EnableAskEnterTreeData) { return false; }

        //    return await dialogService.AskYesNoAsync("Would you like to enter tree data now?", "Sample", false);
        //}

        public void SetStratumFilter(string code)
        {
            SelectedStratumCode = code ?? STRATUM_FILTER_ALL;
        }

        //public void ShowTree(Tree tree)
        //{
        //    if (tree != null)
        //    {
        //        ShowEditTree(tree);
        //    }
        //}

        //private void ShowEditTree(Tree tree)
        //{
        //    ServiceService.DialogService.ShowEditTreeAsync(tree, DataService);
        //}
    }
}