using FScruiser.Models;
using FScruiser.Services;
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
            if (Dataservice.IsPlotNumberAvalible(UnitCode, newValue))
            {
                return true;
            }
            else
            {
                ServiceService.DialogService.ShowMessageAsync("Plot Number Already Takend");
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

        public ICuttingUnitDatastore Dataservice => ServiceService.Datastore;

        public PlotEditViewModel(ServiceService serviceService, string unitCode, int? plotNumber) : base(serviceService)
        {
            UnitCode = unitCode;

            if (plotNumber == null)
            {
                _isAddingPlot = true;
                _plotNumber = Dataservice.GetNextPlotNumber(unitCode);
            }
            else
            {
                _plotNumber = plotNumber.Value;
            }
        }

        public void Init()
        {
            var strata = Strata = Dataservice.GetPlotStrataProxies(UnitCode).ToArray();
            var stratumPlots = new List<StratumPlotViewModel>();

            foreach (var stratum in strata)
            {
                var stratumPlot = new StratumPlotViewModel(ServiceService, UnitCode, stratum.Code, PlotNumber);

                if(_isAddingPlot)
                {
                    stratumPlot.ToggleInCruiseAsync().RunSynchronously();
                }

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

                if(stratumPlot.InCruise)
                {
                    Dataservice.UpdateStratumPlot(stratumPlot);
                }
            }
        }

        private void StratumPlot_InCruiseChanged(object sender, EventArgs e)
        {
            if (sender is StratumPlot stratumPlot && stratumPlot != null)
            {
                if (stratumPlot.InCruise)
                {
                    Dataservice.InsertStratumPlot(UnitCode, stratumPlot);
                }
                else
                {
                    Dataservice.DeleteStratumPlot(stratumPlot.Plot_GUID);
                }
            }
        }

        //public void Save()
        //{
        //    foreach (var stratumPlot in StratumPlots)
        //    {
        //        if (stratumPlot.InCruise)
        //        {
        //            if (stratumPlot.Plot_GUID == null)
        //            {
        //                Dataservice.InsertStratumPlot(UnitCode, stratumPlot);
        //            }
        //            else
        //            {
        //                Dataservice.UpdateStratumPlot(stratumPlot);
        //            }
        //        }
        //        else if (stratumPlot.Plot_GUID != null)
        //        {
        //            Dataservice.DeleteStratumPlot(stratumPlot.Plot_GUID);
        //        }
        //    }
        //}

        public void ShowLimitingDistanceCalculator(StratumPlot stratumPlot)
        {
            //throw new NotImplementedException();
        }
    }
}