using CruiseDAL.DataObjects;
using FScruiser.Logic;
using FScruiser.Models;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class UnitTreeTallyViewModel : ViewModelBase
    {
        public static readonly string STRATUM_FILTER_ALL = "All";

        private ICuttingUnitDataService _dataService;
        private Dictionary<string, IEnumerable<TallyPopulation>> _tallies;
        private string _selectedStratum = STRATUM_FILTER_ALL;
        private IEnumerable<string> _stratumCodes;
        private ICommand _stratumSelectedCommand;
        private IEnumerable<StratumDO> _strata;
        private ICommand _tallyCommand;
        private IEnumerable<string> _strataFilterOptions;

        public Dictionary<string, IEnumerable<TallyPopulation>> Tallies
        {
            get { return _tallies; }
            protected set {
                SetValue(ref _tallies, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
            }
        }

        public IEnumerable<StratumDO> Strata
        {
            get { return _strata; }
            set { SetValue(ref _strata, value); }
        }

        public IEnumerable<string> StrataFilterOptions
        {
            get { return _strataFilterOptions; }
            set { SetValue(ref _strataFilterOptions, value); }
                }

        public IEnumerable<string> StratumCodes
        {
            get { return _stratumCodes; }
            protected set { SetValue(ref _stratumCodes, value); }
        }

        public string StratumFilter
        {
            get { return _selectedStratum; }
            set
            {
                SetValue(ref _selectedStratum, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
            }
        }

        public IEnumerable<TallyPopulation> TalliesFiltered
        {
            get
            {
                if (Tallies == null) { return Enumerable.Empty<TallyPopulation>(); }
                if (StratumFilter == STRATUM_FILTER_ALL)
                {
                    return Tallies.Values.SelectMany(x => x).ToArray();
                }
                else
                {
                    return Tallies[StratumFilter];
                }
            }
        }

        public ICommand TallyCommand => _tallyCommand ?? (_tallyCommand = new Command<TallyPopulation>(this.Tally));
        

        public ObservableCollection<TallyFeedItem> TallyFeed { get; set; } = new ObservableCollection<TallyFeedItem>();

        public ICuttingUnitDataService DataService
        {
            get { return _dataService; }
            protected set { SetValue(ref _dataService, value); }
        }

        public ICommand StratumSelectedCommand => _stratumSelectedCommand
            ?? (_stratumSelectedCommand = new Command<string>(x => SetStratumFilter(x)));

        protected ServiceService ServiceService { get; set; }

        public UnitTreeTallyViewModel(INavigation navigation, ServiceService serviceService) : base(navigation)
        {
            ServiceService = serviceService;
        }

        public void Init(ICuttingUnitDataService dataService)
        {
            dataService.RefreshData();
            DataService = dataService;

            StratumCodes = dataService.Strata.Select(x => x.Code);
            Strata = dataService.Strata;

            var tallyLookup = dataService.TallyPopulations
                .GroupBy(x => x.StratumCode)
                .ToDictionary(x => x.Key, x => (IEnumerable<TallyPopulation>)x.ToArray());
            Tallies = tallyLookup;
            

            StrataFilterOptions = Strata.Select(x => x.Code)
                .Append(STRATUM_FILTER_ALL)
                .ToArray();
        }

        private void Tally(TallyPopulation obj)
        {
            ITallySettingsDataService tallySettings = ServiceService.TallySettingsDataService;
            IDialogService dialogService = ServiceService.DialogService;
            ISoundService soundService = ServiceService.SoundService;
            ICuttingUnitDataService dataService = DataService;

            TreeBasedTallyLogic.OnTally(obj, dataService, TallyFeed, tallySettings, dialogService, soundService);

            //TallyFeed.Add(new TallyFeedItem() { Count = obj, Time = DateTime.Now, Tree = tree });
        }

        public void SetStratumFilter(string code)
        {
            StratumFilter = code ?? STRATUM_FILTER_ALL;
        }

        public void ShowTallyFeedItem(TallyFeedItem selectedItem)
        {
            if (selectedItem == null) { throw new ArgumentNullException(nameof(selectedItem)); }
        }
    }
}