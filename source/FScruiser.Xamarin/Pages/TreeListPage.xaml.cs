using FScruiser.Models;
using FScruiser.Services;
using FScruiser.XF.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;

namespace FScruiser.XF.Pages
{
    public partial class TreeListPage : ContentPage
    {

        public TreeListPage()
        {
            InitializeComponent();

            _goToEndButton.Clicked += _goToEndButton_Clicked;
            _goToStartButton.Clicked += _goToStartButton_Clicked;

            _treeListView.ItemSelected += _treeListView_ItemSelected;

            BindingContext = new TreeListViewModel();

            Appearing += async (sender, ea) =>
            {
                if (BindingContext is TreeListViewModel vm)
                {
                    await vm.InitAsync();
                }
            };
        }

        private void _treeListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (BindingContext is TreeListViewModel vm 
                && e.SelectedItem is Tree tree && tree != null)
            {
                vm.ShowEditTree(tree);
            }

            _treeListView.SelectedItem = null; //deselect selected item
        }

        private void _goToEndButton_Clicked(object sender, EventArgs e)
        {
            var itemSource = _treeListView.ItemsSource;
            if (itemSource == null) { return; }
            var lastItem = itemSource.Cast<object>().LastOrDefault();
            if (lastItem == null) { return; }

            _treeListView.ScrollTo(lastItem, ScrollToPosition.End, true);
        }

        private void _goToStartButton_Clicked(object sender, EventArgs e)
        {
            var itemSource = _treeListView.ItemsSource;
            if (itemSource == null) { return; }
            var firstItem = itemSource.Cast<object>().FirstOrDefault();
            if (firstItem == null) { return; }

            _treeListView.ScrollTo(firstItem, ScrollToPosition.Start, true);
        }
    }
}