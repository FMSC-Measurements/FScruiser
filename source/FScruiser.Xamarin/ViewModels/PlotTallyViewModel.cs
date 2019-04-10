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
    public class PlotTallyViewModel : ViewModelBase
    {
        private const string STRATUM_FILTER_ALL = "All";

        private int _plotNumber;
        private string _unitCode;
        private ICollection<TallyPopulation_Plot> _tallyPopulations;
        private ICollection<StratumProxy> _strata;
        private string _stratumFilter = STRATUM_FILTER_ALL;
        private ICollection<TreeStub_Plot> _trees;
        private ICommand _tallyCommand;
        private Command _editPlotCommand;

        public event EventHandler TreeAdded;

        protected ICuttingUnitDatastore Datastore { get; }
        public IDialogService DialogService { get; }
        public ISampleSelectorDataService SampleSelectorDataService { get; private set; }
        public ITallySettingsDataService TallySettings { get; private set; }
        public ISoundService SoundService { get; private set; }

        public PlotTallyViewModel(INavigationService navigationService
            , IDialogService dialogService
            , ICuttingUnitDatastoreProvider datastoreProvider
            , ISoundService soundService
            , ITallySettingsDataService tallySettings) : base(navigationService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
            SampleSelectorDataService = datastoreProvider.SampleSelectorDataService;
            DialogService = dialogService;
            TallySettings = tallySettings;
            SoundService = soundService;
        }

        public ICommand TallyCommand => _tallyCommand
            ?? (_tallyCommand = new Command<TallyPopulation_Plot>(async (x) => await this.TallyAsync(x)));

        public ICommand EditPlotCommand => _editPlotCommand
            ?? (_editPlotCommand = new Command(() => ShowEditPlot()));

        public int PlotNumber
        {
            get { return _plotNumber; }
            set { SetValue(ref _plotNumber, value); }
        }

        public string UnitCode
        {
            get { return _unitCode; }
            set { SetValue(ref _unitCode, value); }
        }

        public bool IsRecon { get; private set; }

        public ICollection<TallyPopulation_Plot> TallyPopulations
        {
            get { return _tallyPopulations; }
            set
            {
                SetValue(ref _tallyPopulations, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
            }
        }

        public ICollection<TallyPopulation_Plot> TalliesFiltered => TallyPopulations.OrEmpty()
            .Where(x => StratumFilter == STRATUM_FILTER_ALL || x.StratumCode == StratumFilter).ToArray();

        public ICollection<StratumProxy> Strata
        {
            get { return _strata; }
            set
            {
                SetValue(ref _strata, value);
                RaisePropertyChanged(nameof(StrataFilterOptions));
            }
        }

        public ICollection<string> StrataFilterOptions => Strata.OrEmpty().Select(x => x.Code).Append(STRATUM_FILTER_ALL).ToArray();

        public string StratumFilter
        {
            get { return _stratumFilter; }
            set
            {
                SetValue(ref _stratumFilter, value);
                RaisePropertyChanged(nameof(TalliesFiltered));
            }
        }

        public ICollection<TreeStub_Plot> Trees
        {
            get { return _trees; }
            set { SetValue(ref _trees, value); }
        }

        protected void OnTreeAdded()
        {
            TreeAdded?.Invoke(this, null);
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            MessagingCenter.Unsubscribe<object>(this, Messages.EDIT_TREE_CLICKED);
            MessagingCenter.Unsubscribe<object>(this, Messages.DELETE_TREE_CLICKED);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            MessagingCenter.Subscribe<object, string>(this, Messages.EDIT_TREE_CLICKED, (sender, tree_guid) => ShowEditTree(tree_guid));
            MessagingCenter.Subscribe<object, string>(this, Messages.DELETE_TREE_CLICKED, (sender, tree_guid) => DeleteTree(tree_guid));
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var plotID = parameters.GetValue<string>(NavParams.PlotID);

            var unitCode = parameters.GetValue<string>(NavParams.UNIT);
            var plotNumber = parameters.GetValue<int>(NavParams.PLOT_NUMBER);

            Plot plot = null;
            if (string.IsNullOrWhiteSpace(plotID) == false)
            {
                plot = Datastore.GetPlot(plotID);
            }
            else
            {
                plot = Datastore.GetPlot(unitCode, plotNumber);
            }

            var salePurpose = Datastore.GetCruisePurpose();
            IsRecon = salePurpose.ToLower() == "recon";

            TallyPopulations = Datastore.GetPlotTallyPopulationsByUnitCode(plot.CuttingUnitCode, plot.PlotNumber).ToArray();
            Strata = Datastore.GetPlotStrataProxies(UnitCode).ToArray();
            Trees = Datastore.GetPlotTreeProxies(UnitCode, PlotNumber).ToObservableCollection();

            PlotNumber = plot.PlotNumber;
            UnitCode = plot.CuttingUnitCode;
        }


        public async Task TallyAsync(TallyPopulation_Plot pop)
        {
            if (pop.InCruise == false) { return; }

            IDialogService dialogService = DialogService;

            if (pop.IsEmpty)
            {
                await dialogService.ShowMessageAsync("To tally trees, goto plot edit page and unmark stratum as empty", "Stratum Is Marked As Empty");
                return;
            }

            ITallySettingsDataService tallySettings = TallySettings;
            ISoundService soundService = SoundService;
            ICuttingUnitDatastore dataService = Datastore;
            ISampleSelectorDataService sampleSelectorRepository = SampleSelectorDataService;

            var nextTreeNumber = Datastore.GetNextPlotTreeNumber(UnitCode, pop.StratumCode, PlotNumber, IsRecon);

            var tree = await PlotBasedTallyLogic.TallyAsync(pop, UnitCode, PlotNumber, sampleSelectorRepository, dialogService);//TODO async
            if(tree == null) { return; }

            tree.TreeNumber = nextTreeNumber;
            Datastore.InsertTree(tree);
            _trees.Add(tree);
            OnTreeAdded();

            await HandleTally(pop, tree, soundService, dialogService, tallySettings);
        }

        public void ShowEditTree(string tree_guid)
        {
            NavigationService.NavigateAsync("Tree", new NavigationParameters() { { "Tree_Guid", tree_guid } }, useModalNavigation: true);
        }

        public static async Task HandleTally(TallyPopulation_Plot population,
           TreeStub_Plot tree,
           ISoundService soundService,
           IDialogService dialogService,
           ITallySettingsDataService tallySettings)
        {
            if (tree == null) { throw new ArgumentNullException(nameof(tree)); }

            soundService.SignalTally();
            if (tree.CountOrMeasure == "M")
            {
                soundService.SignalMeasureTree();

                //if (tallySettings.EnableCruiserPopup)
                //{
                //    await dialogService.AskCruiserAsync(tree);
                //}
            }
            else if (tree.CountOrMeasure == "I")
            {
                soundService.SignalInsuranceTree();
            }
        }

        public void ShowEditPlot()
        {
            NavigationService.NavigateAsync("PlotEdit", new NavigationParameters($"UnitCode={UnitCode}&PlotNumber={PlotNumber}"));
        }

        public void DeleteTree(string tree_guid)
        {
            Datastore.DeleteTree(tree_guid);
            var tree = Trees.Where(x => x.TreeID == tree_guid).Single();
            Trees.Remove(tree);
        }

        public void DeleteTree(TreeStub_Plot tree)
        {
            Datastore.DeleteTree(tree.TreeID);
            Trees.Remove(tree);
        }

        
    }
}