using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class ManagerCruisersViewModel : ViewModelBase
    {
        private Command<string> _removeCruiserCommand;
        private Command<string> _addCruiserCommand;

        public ITallySettingsDataService DataService { get; }

        public IEnumerable<string> Cruisers => DataService.Cruisers;

        public bool EnableCruiserPopup => DataService.EnableCruiserPopup;

        public ICommand RemoveCruiserCommand => _removeCruiserCommand ?? (_removeCruiserCommand = new Command<string>(RemoveCruiser));

        public ICommand AddCruiserCommand => _addCruiserCommand ?? (_addCruiserCommand = new Command<string>(AddCruiser));



        public ManagerCruisersViewModel(ITallySettingsDataService dataService)
        {
            DataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        public void AddCruiser(string cruiser)
        {
            if (!string.IsNullOrWhiteSpace(cruiser))
            {
                DataService.AddCruiser(cruiser);
                base.RaisePropertyChanged(nameof(Cruisers));
            }
        }

        private void RemoveCruiser(string cruiser)
        {
            DataService.RemoveCruiser(cruiser);
            base.RaisePropertyChanged(nameof(Cruisers));
        }
    }
}