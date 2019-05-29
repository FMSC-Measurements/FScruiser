using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Constants;
using FScruiser.XF.Services;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotEditViewModel : ViewModelBase
    {
        private Plot _plot;
        private IEnumerable<Plot_Stratum> _stratumPlots;
        private Command<Plot_Stratum> _showLimitingDistanceCommand;
        private Command<Plot_Stratum> _toggleInCruiseCommand;
        private IEnumerable<PlotError> _errorsAndWarnings;

        public IEnumerable<PlotError> ErrorsAndWarnings
        {
            get => _errorsAndWarnings;
            set => SetValue(ref _errorsAndWarnings, value);
        }

        public Plot Plot
        {
            get => _plot;
            set
            {
                var oldValue = _plot;
                if (oldValue != null)
                {
                    oldValue.PropertyChanged -= Plot_PropertyChanged;
                }

                SetValue(ref _plot, value);
                RaisePropertyChanged(nameof(PlotNumber));

                if (value != null)
                {
                    value.PropertyChanged += Plot_PropertyChanged;
                }
            }
        }

        public ICommand ShowLimitingDistanceCommand => _showLimitingDistanceCommand ?? (_showLimitingDistanceCommand = new Command<Plot_Stratum>(async x => await ShowLimitingDistanceCalculatorAsync(x)));

        public ICommand ToggleInCruiseCommand => _toggleInCruiseCommand ?? (_toggleInCruiseCommand = new Command<Plot_Stratum>(async (x) => await ToggleInCruiseAsync(x)));

        #region PlotNumber

        public int PlotNumber
        {
            get { return Plot?.PlotNumber ?? 0; }
            set
            {
                var curPlotNumber = Plot?.PlotNumber;
                if (curPlotNumber.HasValue == false || curPlotNumber == value) { return; }
                if (OnPlotNumberChanging(curPlotNumber.Value, value))
                {
                    Plot.PlotNumber = value;
                    OnPlotNumberChanged(value);
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
            RaisePropertyChanged(nameof(PlotNumber));
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

        public string UnitCode => Plot?.CuttingUnitCode;

        public IEnumerable<StratumProxy> Strata { get; set; }

        public IEnumerable<Plot_Stratum> StratumPlots
        {
            get { return _stratumPlots; }
            set
            {
                var oldValue = _stratumPlots;
                if (oldValue != null)
                {
                    foreach (var sp in oldValue)
                    {
                        sp.PropertyChanged -= StratumPlot_PropertyChanged;
                    }
                }

                SetValue(ref _stratumPlots, value);

                if (value != null)
                {
                    foreach (var sp in value)
                    {
                        sp.PropertyChanged += StratumPlot_PropertyChanged;
                    }
                }
            }
        }

        public IPlotDatastore Datastore { get; set; }
        public IDialogService DialogService { get; set; }

        public PlotEditViewModel(ICuttingUnitDatastoreProvider datastoreProvider
            , IDialogService dialogService
            , INavigationService navigationService) : base(navigationService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
            DialogService = dialogService;
        }

        public async Task ToggleInCruiseAsync(Plot_Stratum stratumPlot)
        {
            if (stratumPlot.InCruise)
            {
                var hasTreeData = Datastore.GetNumTreeRecords(UnitCode, stratumPlot.StratumCode, stratumPlot.PlotNumber) > 0;

                if (hasTreeData)
                {
                    if (await DialogService.AskYesNoAsync("Removing stratum will delete all tree data", "Continue?"))
                    {
                        Datastore.DeletePlot_Stratum(stratumPlot.CuttingUnitCode, stratumPlot.StratumCode, stratumPlot.PlotNumber);
                        stratumPlot.InCruise = false;
                    }
                }
                else
                {
                    Datastore.DeletePlot_Stratum(stratumPlot.CuttingUnitCode, stratumPlot.StratumCode, stratumPlot.PlotNumber);
                    stratumPlot.InCruise = false;
                }
            }
            else
            {
                if (stratumPlot.CruiseMethod == CruiseDAL.Schema.CruiseMethods.THREEPPNT)
                {
                    var query = $"{NavParams.UNIT}={stratumPlot.CuttingUnitCode}&{NavParams.PLOT_NUMBER}={stratumPlot.PlotNumber}&{NavParams.STRATUM}={stratumPlot.StratumCode}";

                    await NavigationService.NavigateAsync("ThreePPNTPlot",
                        new NavigationParameters(query));
                }
                else
                {
                    Datastore.InsertPlot_Stratum(stratumPlot);
                    stratumPlot.InCruise = true;
                }
            }

            RefreshErrorsAndWarnings(Plot);
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

            var stratumPlots = Datastore.GetPlot_Strata(plot.CuttingUnitCode, plot.PlotNumber);

            Plot = plot;
            StratumPlots = stratumPlots;

            RefreshErrorsAndWarnings(plot);
        }

        protected void RefreshErrorsAndWarnings(Plot plot)
        {
            var errorsAndWarnings = Datastore.GetPlotErrors(plot.PlotID);
            ErrorsAndWarnings = errorsAndWarnings;
        }

        private void Plot_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var plot = Plot;

            switch (e.PropertyName)
            {
                case nameof(Plot.Aspect):
                case nameof(Plot.Slope):
                case nameof(Plot.Remarks):
                    {
                        Datastore.UpdatePlot(plot);
                        break;
                    }
            }
        }

        private void StratumPlot_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is Plot_Stratum stratumPlot && stratumPlot != null)
            {
                var propertyName = e.PropertyName;
                if (e.PropertyName == nameof(Plot_Stratum.InCruise)) { return; }

                if (stratumPlot.InCruise)
                {
                    Datastore.UpdatePlot_Stratum(stratumPlot);
                }

                RefreshErrorsAndWarnings(Plot);
            }
        }

        public async Task ShowLimitingDistanceCalculatorAsync(Plot_Stratum stratumPlot)
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