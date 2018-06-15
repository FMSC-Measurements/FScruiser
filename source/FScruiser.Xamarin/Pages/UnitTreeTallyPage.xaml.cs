using FScruiser.Models;
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
        protected UnitTreeTallyViewModel ViewModel => (UnitTreeTallyViewModel)BindingContext;

        public UnitTreeTallyPage()
        {
            InitializeComponent();

            BindingContext = new UnitTreeTallyViewModel();
            Appearing += async (sender, ea) =>
            {
                if (BindingContext is UnitTreeTallyViewModel vm)
                {
                    await vm.InitAsync();
                    vm.TallyEntryAdded += TallyFeed_CollectionChanged;
                }
            };

            Disappearing += (x, ea) =>
            {
                if (BindingContext is UnitTreeTallyViewModel vm)
                {
                    vm.TallyEntryAdded -= TallyFeed_CollectionChanged;
                }
            };
        }

        private void TallyFeed_CollectionChanged(object sender, EventArgs e)
        {
            var lastItem = _tallyFeedListView.ItemsSource.OfType<object>().LastOrDefault();
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

        public void _tallyEntryViewCell_editClicked(object sender, TallyEntry tallyEntry)
        {
            if (tallyEntry != null)
            {
                var viewModel = ViewModel;
                viewModel.EditTree(tallyEntry);
            }
        }

        public void _tallyEntryViewCell_untally5Clicked(object sender, TallyEntry tallyEntry)
        {
            if (tallyEntry != null)
            {
                var viewModel = ViewModel;
                viewModel.Untally(tallyEntry);
            }
        }
    }

    public class TreeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TreeItemTemplate { get; set; }
        public DataTemplate BasicTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var tallyEntry = (TallyEntry)item;

            return (tallyEntry.Tree != null) ? TreeItemTemplate : BasicTemplate;
        }
    }
}