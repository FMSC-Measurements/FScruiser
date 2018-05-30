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
        private Command<string> _removeCruiserCommand;

        private ServiceService ServiceService { get; set; }
        public ITallySettingsDataService DataService => ServiceService.TallySettingsDataService;

        public ICommand RemoveCruiserCommand => _removeCruiserCommand ?? (_removeCruiserCommand = new Command<string>(RemoveCruiser));

        public IEnumerable<string> Cruisers => DataService.Cruisers;

        public ManageCruisersPage()
        {
            ServiceService = App.ServiceService;

            InitializeComponent();
            _addCruiserButton.Clicked += _addCruiserButton_Clicked;
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void RefreshCruisers()
        {
            _cruisersListView.ItemsSource = DataService.Cruisers;
        }

        private void _addCruiserButton_Clicked(object sender, EventArgs e)
        {
            var cruiserValue = _addCruiserEntry.Text;
            if (!string.IsNullOrWhiteSpace(cruiserValue))
            {
                DataService.AddCruiser(cruiserValue);
                base.OnPropertyChanged(nameof(Cruisers));
                _addCruiserEntry.Text = "";
            }
        }

        private void RemoveCruiser(string cruiser)
        {
            DataService.RemoveCruiser(cruiser);
            base.OnPropertyChanged(nameof(Cruisers));
        }
    }
}