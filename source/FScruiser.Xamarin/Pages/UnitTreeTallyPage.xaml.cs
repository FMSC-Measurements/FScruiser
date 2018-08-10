using FScruiser.Models;
using FScruiser.Services;
using FScruiser.Util;
using FScruiser.XF.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnitTreeTallyPage : ContentPage
    {
        private bool _treeCellIsSelected;

        protected UnitTreeTallyViewModel ViewModel => (UnitTreeTallyViewModel)BindingContext;

        public UnitTreeTallyPage()
        {
            InitializeComponent();

            Appearing += async (sender, ea) =>
            {
                

            };

            Disappearing += (x, ea) =>
            {
                
            };

            
        }

        public UnitTreeTallyPage(string unitCode,
            ICuttingUnitDatastore datastore,
            IDialogService dialogService,
            ISampleSelectorDataService sampleSelectorDataService,
            ITallySettingsDataService tallySettings,
            ISoundService soundService) : this()
        {
            BindingContext = new UnitTreeTallyViewModel(unitCode,
                datastore,
                dialogService,
                sampleSelectorDataService,
                tallySettings,
                soundService);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is UnitTreeTallyViewModel vm)
            {
                vm.InitAsync().ConfigureAwait(true);
                vm.TallyEntryAdded += TallyFeed_CollectionChanged;
                TallyFeed_CollectionChanged(null, null);
            }

            MessagingCenter.Subscribe<object, TallyEntry>(this, Messages.EDIT_TREE_CLICKED, _tallyEntryViewCell_editClicked);
            MessagingCenter.Subscribe<object, TallyEntry>(this, Messages.UNTALLY_CLICKED, _tallyEntryViewCell_UntallyClicked);
            MessagingCenter.Subscribe<object, bool>(this, Messages.TREECELL_ISELECTED_CHANGED, _tallyEntryViewCell_IsSelectedChanged);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (BindingContext is UnitTreeTallyViewModel vm)
            {
                vm.TallyEntryAdded -= TallyFeed_CollectionChanged;
            }

            MessagingCenter.Unsubscribe<object, TallyEntry>(this, Messages.EDIT_TREE_CLICKED);
            MessagingCenter.Unsubscribe<object, TallyEntry>(this, Messages.UNTALLY_CLICKED);
            MessagingCenter.Unsubscribe<object, bool>(this, Messages.TREECELL_ISELECTED_CHANGED);
        }

        private void TallyFeed_CollectionChanged(object sender, EventArgs e)
        {
            if (_treeCellIsSelected) { return; } //dont scroll down it tree entry is in edit mode

            var lastItem = _tallyFeedListView.ItemsSource.OrEmpty().OfType<object>().LastOrDefault();
            if (lastItem != null)
            {
                _tallyFeedListView.ScrollTo(lastItem, ScrollToPosition.End, false);
            }
        }

        //private void TallyFeedListView_BindingContextChanged(object sender, EventArgs e)
        //{
        //    if (sender == null) { return; }
        //    var view = (ListView)sender;

        //    var tallyFeedCollection = view.BindingContext as INotifyCollectionChanged;
        //    if(tallyFeedCollection == null) { return; }

        //    tallyFeedCollection.CollectionChanged += TallyFeedCollection_CollectionChanged;
        //}

        //private void TallyFeedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    var tallyFeed = sender as IEnumerable<TallyFeedItem>;

        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        _tallyFeedListView.ScrollTo(tallyFeed.LastOrDefault(), ScrollToPosition.End, false);
        //    }
        //}

        public void TallyFeedListView_ItemSelected(object sender, SelectedItemChangedEventArgs eventArgs)
        {
            var selectedItem = (TallyEntry)eventArgs.SelectedItem;

            if (selectedItem != null)
            {
                var viewModel = ViewModel;
                //viewModel.ShowTree(selectedItem.Tree);
            }

            var view = (ListView)sender;
            //view.SelectedItem = null;//disable selection so that selection acts as a click
        }

        void _tallyEntryViewCell_IsSelectedChanged(object sender, bool isSelected)
        {
            _treeCellIsSelected = isSelected;
        }


        void _tallyEntryViewCell_UntallyClicked(object sender, TallyEntry tallyEntry)
        {
            if(tallyEntry != null)
            {
                ViewModel.Untally(tallyEntry);
                _treeCellIsSelected = false;
            }
        }

        void _tallyEntryViewCell_editClicked(object sender, TallyEntry tallyEntry)
        {
            if (tallyEntry != null)
            {
                var viewModel = ViewModel;
                viewModel.EditTree(tallyEntry);
            }
        }
    }

    
}