using FScruiser.Logic;
using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private IEnumerable<TallyEntry> _tallyFeed;

        public event EventHandler TallyEntryAdded;

        public IEnumerable<TallyEntry> TallyFeed
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

        public override async Task InitAsync()
        {
            var dataService = DataService;
            if (DataService != null)
            {
                await dataService.RefreshDataAsync();

                var tallyLookup = dataService.TallyPopulations
                    .GroupBy(x => x.StratumCode)
                    .ToDictionary(x => x.Key, x => (IEnumerable<TallyPopulation>)x.ToArray());
                Tallies = tallyLookup;

                TallyFeed = dataService.TallyFeed;
            }
        }

        private async Task TallyAsync(TallyPopulation obj)
        {
            ITallySettingsDataService tallySettings = ServiceService.TallySettingsDataService;
            IDialogService dialogService = ServiceService.DialogService;
            ISoundService soundService = ServiceService.SoundService;
            ICuttingUnitDataService dataService = DataService;

            var entry = await TreeBasedTallyLogic.TallyAsync(obj, dataService, dialogService);//TODO async

            dataService.AddTallyEntry(entry);
            RaiseTallyEntryAdded();

            await HandleTally(entry, dataService, soundService, dialogService, tallySettings);

            //TallyFeed.Add(new TallyFeedItem() { Count = obj, Time = DateTime.Now, Tree = tree });
        }

        public static async Task HandleTally(TallyEntry entry,
            ICuttingUnitDataService dataService,
            ISoundService soundService,
            IDialogService dialogService,
            ITallySettingsDataService tallySettings)
        {
            if (entry == null) { throw new ArgumentNullException(nameof(entry)); }

            soundService.SignalTally();
            var tree = entry.Tree;
            if (tree != null)
            {
                if (tree.CountOrMeasure == "M")
                {
                    soundService.SignalMeasureTree();
                }
                else if (tree.CountOrMeasure == "I")
                {
                    soundService.SignalInsuranceTree();
                }

                if (tallySettings.EnableCruiserPopup)
                {
                    await dialogService.AskCruiserAsync(tree);
                    await dataService.UpdateTreeAsync(tree);
                }
                else
                {
                    var sampleType = (tree.CountOrMeasure == "M") ? "Measure Tree" :
                                (tree.CountOrMeasure == "I") ? "Insurance Tree" : String.Empty;
                    await dialogService.ShowMessageAsync("Tree #" + tree.TreeNumber.ToString(), sampleType);
                }

                if (tree.CountOrMeasure == "M" && await AskEnterMeasureTreeDataAsync(tallySettings, dialogService))
                {
                    var task = dialogService.ShowEditTreeAsync(tree, dataService);//allow method to contiue from show edit tree we will allow tally history action to be added in the background
                }
            }
        }

        protected static async Task<bool> AskEnterMeasureTreeDataAsync(ITallySettingsDataService appSettings, IDialogService dialogService)
        {
            if (!appSettings.EnableAskEnterTreeData) { return false; }

            return await dialogService.AskYesNoAsync("Would you like to enter tree data now?", "Sample", false);
        }

        public void SetStratumFilter(string code)
        {
            SelectedStratumCode = code ?? STRATUM_FILTER_ALL;
        }

        public void ShowTree(Tree tree)
        {
            if (tree != null)
            {
                ShowEditTree(tree);
            }
        }

        private void ShowEditTree(Tree tree)
        {
            ServiceService.DialogService.ShowEditTreeAsync(tree, DataService);
        }
    }
}