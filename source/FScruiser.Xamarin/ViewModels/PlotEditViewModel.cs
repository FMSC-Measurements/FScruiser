using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Services;
using FScruiser.XF.Util;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotEditViewModel : ViewModelBase
    {
        private bool _isAddingPlot;
        private int _plotNumber;
        private IEnumerable<StratumPlotViewModel> _stratumPlots;
        private Command<StratumPlot> _showLimitingDistanceCommand;

        public ICommand ShowLimitingDistanceCommand => _showLimitingDistanceCommand ?? (_showLimitingDistanceCommand = new Command<StratumPlot>(ShowLimitingDistanceCalculator));

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
                    stratumPlot.StratumPlot.PlotNumber = plotNumber;
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

        public IEnumerable<StratumPlotViewModel> StratumPlots
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


        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            LoadData(parameters);
            base.OnNavigatedTo(parameters);
        }

        private void LoadData(NavigationParameters parameters)
        {
            var unitCode = UnitCode = parameters.GetValue<string>("UnitCode");
            var plotNumber = parameters.GetValueOrDefault<int>("PlotNumber");
            var isAddingPlot = parameters.GetValueOrDefault<bool>("IsAddingPlot");

            if (isAddingPlot)
            {
                plotNumber = Datastore.GetNextPlotNumber(unitCode);
            }

            PlotNumber = plotNumber;

            var strata = Strata = Datastore.GetPlotStrataProxies(UnitCode).ToArray();
            var stratumPlots = new List<StratumPlotViewModel>();

            foreach (var stratum in strata)
            {
                var stratumPlot = new StratumPlotViewModel(Datastore, DialogService, UnitCode, stratum.Code, PlotNumber, isAddingPlot);

                stratumPlots.Add(stratumPlot);
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

        private void StratumPlot_InCruiseChanged(object sender, EventArgs e)
        {
            if (sender is StratumPlot stratumPlot && stratumPlot != null)
            {
                if (stratumPlot.InCruise)
                {
                    Datastore.InsertStratumPlot(UnitCode, stratumPlot);
                }
                else
                {
                    Datastore.DeleteStratumPlot(stratumPlot.Plot_GUID);
                }
            }
        }

        public void ShowLimitingDistanceCalculator(StratumPlot stratumPlot)
        {
            NavigationService.NavigateAsync("LimitingDistance", new NavigationParameters($"UnitCode={UnitCode}&PlotNumber={stratumPlot.PlotNumber}&StratumCode={stratumPlot.StratumCode}"));
        }
    }
}