using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FScruiser.Models;
using FScruiser.Pages;
using FScruiser.Services;
using FScruiser.XF.Pages;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class PlotListViewModel : ViewModelBase
    {
        private ICommand _addPlotCommand;
        private Command<Plot> _editPlotCommand;

        public string UnitCode { get; set; }

        public IEnumerable<Plot> Plots { get; protected set; }

        public ICuttingUnitDatastore Datastore => ServiceService.Datastore;

        public ICommand AddPlotCommand => _addPlotCommand ?? (_addPlotCommand = new Command(AddPlot));

        public ICommand EditPlotCommand => _editPlotCommand ?? (_editPlotCommand = new Command<Plot>(ShowEditPlot));

        public INavigation NavigationService { get; set; }
        

        public PlotListViewModel(INavigation navigationService, ServiceService serviceService, string unitCode) : base(serviceService)
        {
            NavigationService = navigationService;
            UnitCode = unitCode;
        }

        public void Init()
        {
            Plots = Datastore.GetPlotsByUnitCode(UnitCode).ToArray();
            RaisePropertyChanged(nameof(Plots));
        }

        public void AddPlot(object obj)
        {
            //show plot edit dialog with plot number = null
            var view = new PlotEditPage(ServiceService, UnitCode, (int?)null);

            NavigationService.PushAsync(view);
        }

        public void ShowEditPlot(Plot plot)
        {
            var view = new PlotEditPage(ServiceService, UnitCode, plot.PlotNumber);

            NavigationService.PushAsync(view);
        }

        public void ShowTallyPlot(Plot plot)
        {
            var view = new PlotTallyPage(ServiceService, UnitCode, plot.PlotNumber);

            NavigationService.PushAsync(view);
        }
    }
}
