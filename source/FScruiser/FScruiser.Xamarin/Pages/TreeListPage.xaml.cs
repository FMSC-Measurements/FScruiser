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
        public TreeListPage()
        {
            InitializeComponent();
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