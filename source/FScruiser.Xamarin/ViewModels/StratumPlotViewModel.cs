using FScruiser.Models;
using FScruiser.Services;
using FScruiser.Util;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class StratumPlotViewModel : INPC_Base
    {
        private bool _inCruise;
        private StratumPlot _stratumPlot;
        private Command _toggleInCruiseCommand;
        private bool _isToggleingInCruise;

        ICuttingUnitDatastore Datastore { get; }
        IDialogService DialogService { get; }

        public ICommand ToggleInCruiseCommand => _toggleInCruiseCommand ?? (_toggleInCruiseCommand = new Command(async () => await ToggleInCruiseAsync()));

        public StratumPlotViewModel(ICuttingUnitDatastore datastore
            , IDialogService dialogService
            , string unitCode
            , string stratumCode
            , int plotNumber
            , bool isAddingPlot)
        {
            Datastore = datastore;
            DialogService = dialogService;

            UnitCode = unitCode;

            StratumPlot = Datastore.GetStratumPlot(unitCode, stratumCode, plotNumber, isAddingPlot);
        }

        public StratumPlot StratumPlot
        {
            get { return _stratumPlot; }
            set
            {
                if(_stratumPlot != null)
                {
                    _stratumPlot.PropertyChanged -= StratumPlot_PropertyChanged;
                }
                SetValue(ref _stratumPlot, value);
                if(value != null)
                {
                    value.PropertyChanged += StratumPlot_PropertyChanged;
                }
                RaisePropertyChanged(nameof(InCruise));
            }
        }

        public bool InCruise => StratumPlot?.InCruise ?? false;
        //{
        //    get { return _inCruise; }
        //    set
        //    {
        //        SetValue(ref _inCruise, value);
        //    }
        //}

        public bool IsEmpty
        {
            get { return StratumPlot.IsEmpty == "True"; }
            set
            {
                StratumPlot.IsEmpty = (value) ? "True" : "False";
            }
        }

        public string UnitCode { get; }

        private void OnInCruiseChanged()
        {
            if (InCruise)
            {
                Datastore.InsertStratumPlot(UnitCode, StratumPlot);
            }
            else
            {
                Datastore.DeleteStratumPlot(StratumPlot.Plot_GUID);
            }
        }

        public async Task ToggleInCruiseAsync()
        {
            if(_isToggleingInCruise) { return; }
            _isToggleingInCruise = true;
            try
            {
                if (InCruise)
                {
                    var hasTreeData = Datastore.GetNumTreeRecords(UnitCode, StratumPlot.StratumCode, StratumPlot.PlotNumber) > 0;

                    if (hasTreeData)
                    {
                        if (await DialogService.AskYesNoAsync("Removing stratum will delete all tree data", "Continue?"))
                        {
                            Datastore.DeleteStratumPlot(StratumPlot.Plot_GUID);
                            _inCruise = false;
                        }
                    }
                    else
                    {
                        Datastore.DeleteStratumPlot(StratumPlot.Plot_GUID);
                        _inCruise = false;
                    }
                }
                else
                {
                    Datastore.InsertStratumPlot(UnitCode, StratumPlot);

                    _inCruise = true;
                }
                RaisePropertyChanged(nameof(InCruise));
            }
            finally
            {
                _isToggleingInCruise = false;
            }
        }

        private void StratumPlot_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is StratumPlot stratumPlot && stratumPlot != null)
            {
                var propertyName = e.PropertyName;
                if (e.PropertyName == nameof(StratumPlot.InCruise)) { return; }

                if (InCruise)
                {
                    Datastore.UpdateStratumPlot(stratumPlot);
                }
            }
        }
    }
}