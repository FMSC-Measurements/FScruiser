using FScruiser.Models;
using FScruiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.Pages
{
    public partial class StratumTalliesPage : ContentPage
    {
        private bool _plotPickerUpdating;
        private StratumTalliesViewModel _viewModel;

        public StratumTalliesPage()
        {
            InitializeComponent();
        }

        public StratumTalliesViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel == value) { return; }
                OnViewModelChanging(_viewModel);
                _viewModel = value;
                OnViewModelChanged(_viewModel);
            }
        }

        private void OnViewModelChanged(StratumTalliesViewModel viewModel)
        {
            //if stratum is a plot stratum
            if (viewModel != null)
            {
                if (viewModel.UnitStratum.Stratum.IsPlotStratum)
                {
                    viewModel.PlotsChanged += viewModel_PlotsChanged;

                    //enable the plot nav bar
                    PlotNavBar.IsVisible = true;

                    UpdatePlotPicker(viewModel);
                }
                else
                {
                    //otherwise hide it
                    PlotNavBar.IsVisible = false;
                }
            }
        }

        private void OnViewModelChanging(StratumTalliesViewModel viewModel)
        {
            if (viewModel != null)
            {
                viewModel.PlotsChanged -= viewModel_PlotsChanged;
            }
        }

        private void viewModel_PlotsChanged(object sender, EventArgs e)
        {
            UpdatePlotPicker(ViewModel);
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private void PlotPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_plotPickerUpdating) { return; }
            var pickerValue = PlotPicker.Items.ElementAtOrDefault(PlotPicker.SelectedIndex);

            //get view model
            var viewModel = (StratumTalliesViewModel)BindingContext;
            viewModel.CurrentPlot = viewModel.UnitStratum.Plots.Where(plot => plot.ToString() == pickerValue).FirstOrDefault();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            //set view model
            ViewModel = BindingContext as StratumTalliesViewModel;
        }

        private void UpdatePlotPicker(StratumTalliesViewModel viewModel)
        {
            try
            {
                _plotPickerUpdating = true;
                PlotPicker.Items.Clear();

                //setup plot picker
                foreach (var plot in viewModel.UnitStratum.Plots)
                {
                    PlotPicker.Items.Add(plot.ToString());
                }

                var index = PlotPicker.Items.IndexOf(viewModel.CurrentPlot?.ToString());
                PlotPicker.SelectedIndex = index;
            }
            finally
            {
                _plotPickerUpdating = false;
            }
        }
    }
}