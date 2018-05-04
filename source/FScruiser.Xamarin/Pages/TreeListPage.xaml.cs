using FScruiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.Pages
{
    public partial class TreeListPage : ContentPage
    {
        private object _listViewFirst;
        private object _listViewLast;

        public TreeListPage()
        {
            InitializeComponent();

            GoToEndButton.Clicked += GoToEndButton_Clicked;
            GoToTopButton.Clicked += GoToTopButton_Clicked;
        }

        private void GoToEndButton_Clicked(object sender, EventArgs e)
        {
            ListView.ScrollTo(
                ListView.ItemsSource.Cast<object>().LastOrDefault(),
                ScrollToPosition.End, false);
        }

        private void GoToTopButton_Clicked(object sender, EventArgs e)
        {
            ListView.ScrollTo(
                ListView.ItemsSource.Cast<object>().FirstOrDefault(),
                ScrollToPosition.Start, false);
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