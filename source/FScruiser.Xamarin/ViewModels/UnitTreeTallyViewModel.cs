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
        private ObservableCollection<TallyFeedItem> _tallyFeed;

        public ObservableCollection<TallyFeedItem> TallyFeed
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
            ?? (_tallyCommand = new Command<TallyPopulation>(this.Tally));

        public ICommand StratumSelectedCommand => _stratumSelectedCommand
            ?? (_stratumSelectedCommand = new Command<string>(x => SetStratumFilter(x)));

        public ICuttingUnitDataService DataService => ServiceService.CuttingUnitDataSercie;

        public UnitTreeTallyViewModel()
        {
            MessagingCenter.Subscribe<object>(this, Messages.CUTTING_UNIT_SELECTED, async (x) =>
            {
                await InitAsync();
            });
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

                TallyFeed = new ObservableCollection<TallyFeedItem>(dataService.TallyFeed);
            }
        }

        private void Tally(TallyPopulation obj)
        {
            ITallySettingsDataService tallySettings = ServiceService.TallySettingsDataService;
            IDialogService dialogService = ServiceService.DialogService;
            ISoundService soundService = ServiceService.SoundService;
            ICuttingUnitDataService dataService = DataService;

            TreeBasedTallyLogic.OnTallyAsync(obj, dataService, TallyFeed, tallySettings, dialogService, soundService);//TODO async

            //TallyFeed.Add(new TallyFeedItem() { Count = obj, Time = DateTime.Now, Tree = tree });
        }

        public void SetStratumFilter(string code)
        {
            SelectedStratumCode = code ?? STRATUM_FILTER_ALL;
        }

        public void ShowTallyFeedItem(TallyFeedItem selectedItem)
        {
            if (selectedItem == null) { throw new ArgumentNullException(nameof(selectedItem)); }
            var tree = selectedItem.Tree;

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