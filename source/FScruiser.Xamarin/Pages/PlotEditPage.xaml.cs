using FScruiser.Services;
using FScruiser.XF.ViewModels;

using Xamarin.Forms;

namespace FScruiser.Pages
{
    public partial class PlotEditPage : ContentPage
    {
        public PlotEditPage()
        {
            InitializeComponent();
        }

        public PlotEditPage(ServiceService serviceService, string unitCode, int? plotNumber) : this()
        {
            var viewModel = new PlotEditViewModel(serviceService, unitCode, plotNumber);
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is PlotEditViewModel viewModel)
            {
                viewModel.Init();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //if (BindingContext is PlotEditViewModel viewModel)
            //{
            //    viewModel.Save();
            //}
        }
    }
}