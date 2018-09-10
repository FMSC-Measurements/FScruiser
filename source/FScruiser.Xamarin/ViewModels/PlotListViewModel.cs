using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.Services;
using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotListViewModel : ViewModelBase
    {
        private ICommand _addPlotCommand;
        private Command<Plot> _editPlotCommand;

        public string UnitCode { get; set; }

        public IEnumerable<Plot> Plots { get; protected set; }

        public ICuttingUnitDatastore Datastore { get; set; }

        public ICommand AddPlotCommand => _addPlotCommand ?? (_addPlotCommand = new Command(AddPlot));

        public ICommand EditPlotCommand => _editPlotCommand ?? (_editPlotCommand = new Command<Plot>(ShowEditPlot));

        public PlotListViewModel(INavigationService navigationService
            , ICuttingUnitDatastoreProvider datastoreProvider) : base(navigationService)
        {
            Datastore = datastoreProvider.CuttingUnitDatastore;
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            if (UnitCode == null) //don't reload param if navigating backwards
            {
                UnitCode = parameters.GetValue<string>("UnitCode");
            }

            Plots = Datastore.GetPlotsByUnitCode(UnitCode).ToArray();
            RaisePropertyChanged(nameof(Plots));

            base.OnNavigatedTo(parameters);
        }

        public void AddPlot(object obj)
        {
            NavigationService.NavigateAsync($"PlotEdit?UnitCode={UnitCode}&IsAddingPlot=true");
        }

        public void ShowEditPlot(Plot plot)
        {
            NavigationService.NavigateAsync($"PlotEdit?UnitCode={UnitCode}&PlotNumber={plot.PlotNumber}");
        }

        public void ShowTallyPlot(Plot plot)
        {
            NavigationService.NavigateAsync($"PlotTally?UnitCode={UnitCode}&PlotNumber={plot.PlotNumber}");
        }
    }
}