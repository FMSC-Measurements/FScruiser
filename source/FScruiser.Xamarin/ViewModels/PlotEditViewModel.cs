using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using FScruiser.XF.Util;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotEditViewModel : ViewModelBase
    {
        private int _plotNumber;
        private IEnumerable<StratumPlot> _stratumPlots;
        private Command<StratumPlot> _showLimitingDistanceCommand;
        private Command<StratumPlot> _toggleInCruiseCommand;

        public ICommand ShowLimitingDistanceCommand => _showLimitingDistanceCommand ?? (_showLimitingDistanceCommand = new Command<StratumPlot>(async x => await ShowLimitingDistanceCalculatorAsync(x)));

        public ICommand ToggleInCruiseCommand => _toggleInCruiseCommand ?? (_toggleInCruiseCommand = new Command<StratumPlot>(async (x) => await ToggleInCruiseAsync(x)));

        #region PlotNumber

        public int PlotNumber
        {
            get { return _plotNumber; }
            set
            {
                if (_plotNumber == value) { return; }
                if (OnPlotNumberChanging(_plotNumber, value))
                {
                    SetValue(ref _plotNumber, value);
                    OnPlotNumberChanged(_plotNumber);
                }
            }
        }

        private void OnPlotNumberChanged(int plotNumber)
        {
            var stratumPlots = StratumPlots;
            if (stratumPlots != null)
            {
                foreach (var stratumPlot in stratumPlots)
                {
                    stratumPlot.PlotNumber = plotNumber;
                }
            }
        }

        private bool OnPlotNumberChanging(int oldValue, int newValue)
        {
            if (Datastore.IsPlotNumberAvalible(UnitCode, newValue))
            {
                return true;
            }
            else
            {
                DialogService.ShowMessageAsync("Plot Number Already Takend");
                return false;
            }
        }

        #endregion PlotNumber

        public string UnitCode { get; set; }

        public IEnumerable<StratumProxy> Strata { get; set; }

        public IEnumerable<StratumPlot> StratumPlots
        {
            get { return _stratumPlots; }
            set
            {
                SetValue(ref _stratumPlots, value);
            }
        }

        public ICuttingUnitDatastore Datastore { get; set; }
        public IDialogService DialogService { get; set; }

        public PlotEditViewModel(ICuttingUnitDatastoreProvider datastoreProvider
            , IDialogService dialogService
            , INavigationService navigationService) : base(navigationService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
            DialogService = dialogService;
        }

        public async Task ToggleInCruiseAsync(StratumPlot stratumPlot)
        {
            if (stratumPlot.InCruise)
            {
                var hasTreeData = Datastore.GetNumTreeRecords(UnitCode, stratumPlot.StratumCode, stratumPlot.PlotNumber) > 0;

                if (hasTreeData)
                {
                    if (await DialogService.AskYesNoAsync("Removing stratum will delete all tree data", "Continue?"))
                    {
                        Datastore.DeleteStratumPlot(stratumPlot.Plot_GUID);
                        stratumPlot.InCruise = false;
                    }
                }
                else
                {
                    Datastore.DeleteStratumPlot(stratumPlot.Plot_GUID);
                    stratumPlot.InCruise = false;
                }
            }
            else
            {
                if (stratumPlot.CruiseMethod == CruiseDAL.Schema.CruiseMethods.THREEPPNT)
                {
                    var query = $"{NavParams.UNIT}={stratumPlot.UnitCode}&{NavParams.PLOT_NUMBER}={stratumPlot.PlotNumber}&{NavParams.STRATUM}={stratumPlot.StratumCode}";

                    await NavigationService.NavigateAsync("ThreePPNTPlot",
                        new NavigationParameters(query));
                }
                else
                {
                    Datastore.InsertStratumPlot(UnitCode, stratumPlot);
                    stratumPlot.InCruise = true;
                }
            }
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            var unitCode = UnitCode = parameters.GetValue<string>("UnitCode");
            var plotNumber = parameters.GetValueOrDefault<int>("PlotNumber");
            var isAddingPlot = parameters.GetValueOrDefault<bool>("IsAddingPlot");

            if (isAddingPlot)
            {
                plotNumber = Datastore.GetNextPlotNumber(unitCode);
            }
            else
            {
            }

            _plotNumber = plotNumber;
            RaisePropertyChanged(nameof(PlotNumber));

            //var strata = Strata = Datastore.GetPlotStrataProxies(UnitCode).ToArray();
            //var stratumPlots = new List<StratumPlotViewModel>();

            //foreach (var stratum in strata)
            //{
            //    var stratumPlot = new StratumPlotViewModel(Datastore, DialogService, UnitCode, stratum.Code, PlotNumber, isAddingPlot);

            //    stratumPlots.Add(stratumPlot);
            //}

            var stratumPlots = Datastore.GetStratumPlots(unitCode, plotNumber, isAddingPlot);

            foreach (var stratumPlot in stratumPlots)
            {
                stratumPlot.PropertyChanged += StratumPlot_PropertyChanged;
            }

            StratumPlots = stratumPlots;
        }

        private void StratumPlot_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is StratumPlot stratumPlot && stratumPlot != null)
            {
                var propertyName = e.PropertyName;
                if (e.PropertyName == nameof(StratumPlot.InCruise)) { return; }

                if (stratumPlot.InCruise)
                {
                    Datastore.UpdateStratumPlot(stratumPlot);
                }
            }
        }

        public async Task ShowLimitingDistanceCalculatorAsync(StratumPlot stratumPlot)
        {
            try
            {
                var navResult = await NavigationService.NavigateAsync("LimitingDistance", new NavigationParameters($"UnitCode={UnitCode}&PlotNumber={stratumPlot.PlotNumber}&StratumCode={stratumPlot.StratumCode}"));

                if (navResult != null)
                {
                    Debug.WriteLine(navResult.Success);
                    if (navResult.Exception != null)
                    {
                        Debug.WriteLine(navResult.Exception);
                    }
                }
            }
            catch (Exception e)
            {
                App.LogException("Navigation", $"Navigating to LimitingDistance", e);
            }
        }
    }
}