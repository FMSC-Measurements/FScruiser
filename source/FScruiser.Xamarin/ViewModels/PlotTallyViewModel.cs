using FScruiser.Logic;
using FScruiser.Models;
using FScruiser.Pages;
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

        protected ICuttingUnitDatastore Datastore => ServiceService.Datastore;

        public PlotTallyViewModel(ServiceService serviceService, INavigation navigation, string unitCode, int plotNumber) : base(serviceService)
        {
            UnitCode = unitCode;
            PlotNumber = plotNumber;
            Navigation = navigation;
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

        public INavigation Navigation { get; }

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

        public void Init()
        {
            var unitCode = UnitCode;
            var plotNumber = PlotNumber;

            var salePurpose = Datastore.GetCruisePurpose();
            IsRecon = salePurpose.ToLower() == "recon";

            TallyPopulations = Datastore.GetPlotTallyPopulationsByUnitCode(unitCode, plotNumber).ToArray();
            Strata = Datastore.GetPlotStrataProxies(unitCode).ToArray();
            Trees = Datastore.GetPlotTreeProxies(unitCode, plotNumber).ToObservableCollection();
        }

        public async Task TallyAsync(TallyPopulation_Plot pop)
        {
            if(pop.InCruise == false) { return; }

            IDialogService dialogService = ServiceService.DialogService;

            if (pop.IsEmptyBool)
            {
                await dialogService.ShowMessageAsync("To tally trees, goto plot edit page and unmark stratum as empty", "Stratum Is Marked As Empty");
                return;
            }

            ITallySettingsDataService tallySettings = ServiceService.TallySettingsDataService;
            
            ISoundService soundService = ServiceService.SoundService;
            ICuttingUnitDatastore dataService = Datastore;
            ISampleSelectorDataService sampleSelectorRepository = ServiceService.SampleSelectorRepository;

            var nextTreeNumber = Datastore.GetNextPlotTreeNumber(UnitCode, pop.StratumCode, PlotNumber, IsRecon);

            var tree = await PlotBasedTallyLogic.TallyAsync(pop, UnitCode, PlotNumber, sampleSelectorRepository, dialogService);//TODO async

            tree.TreeNumber = nextTreeNumber;
            Datastore.InsertTree(tree);
            _trees.Add(tree);
            OnTreeAdded();

            await HandleTally(pop, tree, soundService, dialogService, tallySettings);
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
            var view = new PlotEditPage(ServiceService, UnitCode, PlotNumber);

            Navigation.PushAsync(view);
        }

        public void DeleteTree(string tree_guid)
        {
            Datastore.DeleteTree(tree_guid);
            var tree = Trees.Where(x => x.Tree_GUID == tree_guid).Single();
            Trees.Remove(tree);
        }

        public void DeleteTree(TreeStub_Plot tree)
        {
            Datastore.DeleteTree(tree.Tree_GUID);
            Trees.Remove(tree);
        }
    }
}