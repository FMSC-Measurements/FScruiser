using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageCruisersPage : ContentPage
    {
        public ManageCruisersPage()
        {
            InitializeComponent();
            _addCruiserButton.Clicked += _addCruiserButton_Clicked;
        }

        private void _addCruiserButton_Clicked(object sender, EventArgs e)
        {
            _addCruiserEntry.Text = null;
            //var cruiserValue = _addCruiserEntry.Text;
            //if (!string.IsNullOrWhiteSpace(cruiserValue))
            //{
            //    DataService.AddCruiser(cruiserValue);
            //    base.OnPropertyChanged(nameof(Cruisers));
            //    _addCruiserEntry.Text = "";
            //}
        }
    }
}