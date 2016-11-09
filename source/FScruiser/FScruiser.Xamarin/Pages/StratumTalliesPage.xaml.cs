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
        public StratumTalliesPage()
        {
            InitializeComponent();
        }

        #region CurrentPlot

        /// <summary>
        /// Identifies the <see cref="CurrentPlot"/> bindable property.
        /// </summary>
        public static readonly BindableProperty CurrentPlotProperty =
            BindableProperty.Create(nameof(CurrentPlot),
              typeof(Plot),
              typeof(StratumTalliesPage),
              defaultValue: default(Plot),
              defaultBindingMode: BindingMode.OneWayToSource);

        /// <summary>
        /// Gets or sets the <see cref="CurrentPlot" /> property. This is a bindable property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        public Plot CurrentPlot
        {
            get { return (Plot)GetValue(CurrentPlotProperty); }
            set { SetValue(CurrentPlotProperty, value); }
        }

        #endregion CurrentPlot

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private void PlotPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var pickerValue = PlotPicker.Items.ElementAtOrDefault(PlotPicker.SelectedIndex);

            //get view model
            var viewModel = (StratumTalliesViewModel)BindingContext;
            CurrentPlot = viewModel.UnitStratum.Plots.Where(plot => plot.ToString() == pickerValue).FirstOrDefault();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext == null) { return; } //page popped

            //get view model
            var viewModel = (StratumTalliesViewModel)BindingContext;

            //if stratum is a plot stratum
            if (viewModel.UnitStratum.Stratum.IsPlotStratum)
            {
                //enable the plot nav bar
                PlotNavBar.IsVisible = true;

                //setup plot picker
                foreach (var plot in viewModel.UnitStratum.Plots)
                {
                    PlotPicker.Items.Add(plot.ToString());
                }
                if (PlotPicker.Items.Count > 0)
                {
                    PlotPicker.SelectedIndex = PlotPicker.Items.Count - 1;
                }
            }
            else
            {
                //otherwise hide it
                PlotNavBar.IsVisible = false;
            }
        }
    }
}