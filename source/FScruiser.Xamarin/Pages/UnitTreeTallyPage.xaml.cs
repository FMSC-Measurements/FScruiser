﻿using FScruiser.Models;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnitTreeTallyPage : ContentPage
    {
        private INotifyCollectionChanged _tallyFeedCollection;

        protected UnitTreeTallyViewModel ViewModel => (UnitTreeTallyViewModel)BindingContext;

        public UnitTreeTallyPage()
        {
            InitializeComponent();

            _tallyFeedListView.BindingContextChanged += TallyFeedListView_BindingContextChanged;
        }

        private void TallyFeedListView_BindingContextChanged(object sender, EventArgs e)
        {
            if (sender == null) { return; }
            var view = (ListView)sender;


            var tallyFeedCollection = view.BindingContext as INotifyCollectionChanged;
            if(tallyFeedCollection == null) { return; }

            tallyFeedCollection.CollectionChanged += TallyFeedCollection_CollectionChanged;
        }

        private void TallyFeedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var tallyFeed = sender as IEnumerable<TallyFeedItem>;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                _tallyFeedListView.ScrollTo(tallyFeed.LastOrDefault(), ScrollToPosition.End, false);
            }
        }

        public void TallyFeedListView_ItemSelected(object sender, SelectedItemChangedEventArgs eventArgs)
        {
            var selectedItem = (TallyFeedItem)eventArgs.SelectedItem;

            if (selectedItem != null)
            {
                var viewModel = ViewModel;
                viewModel.ShowTallyFeedItem(selectedItem);
            }

            var view = (ListView)sender;
            view.SelectedItem = null;//disable selection so that selection acts as a click
        }
    }
}