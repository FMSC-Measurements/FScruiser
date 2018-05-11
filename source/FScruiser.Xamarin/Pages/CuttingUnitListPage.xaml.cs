﻿using FScruiser.Models;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FScruiser.XF.Pages
{
    public partial class CuttingUnitListPage : ContentPage
    {
        protected CuttingUnitListViewModel ViewModel => (CuttingUnitListViewModel)BindingContext;

        public CuttingUnitListPage()
        {
            InitializeComponent();

            UnitListView.ItemSelected += UnitListView_ItemSelected;

            BindingContext = new CuttingUnitListViewModel();
        }

        private void UnitListView_ItemSelected(object sender, SelectedItemChangedEventArgs eventArgs)
        {
            if (sender == null) { throw new ArgumentNullException(nameof(sender)); }
            if (eventArgs == null) { throw new ArgumentNullException(nameof(eventArgs)); }
            if(eventArgs.SelectedItem == null) { return; } //selected item may be null, do nothing if it is

            var unit = (CuttingUnit)eventArgs.SelectedItem;

            ViewModel.SelectUnit(unit);

            
            ((ListView)sender).SelectedItem = null;
        }
    }
}