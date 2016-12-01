using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.Pages
{
    public partial class TreeListPage : ContentPage
    {
        private object _listViewFirst;
        private object _listViewLast;

        public TreeListPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            _listViewLast = ListView.ItemsSource?.Cast<object>().LastOrDefault();
            _listViewFirst = ListView.ItemsSource?.Cast<object>().LastOrDefault();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            ListView.ScrollTo(
                ListView.ItemsSource.Cast<object>().LastOrDefault(),
                ScrollToPosition.End, false);

            //TreeScrollView.ScrollToAsync(ListView, ScrollToPosition.End, false);
        }

        //public event EventHandler<Tree> TreeSelected;

        //private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    try
        //    {
        //        var selectedTree = ((ListView)sender).SelectedItem as Tree;
        //        if (selectedTree != null)
        //        {
        //            TreeSelected?.Invoke(this, selectedTree);
        //        }
        //    }
        //    finally
        //    {
        //        ((ListView)sender).SelectedItem = null;
        //    }
        //}
    }
}