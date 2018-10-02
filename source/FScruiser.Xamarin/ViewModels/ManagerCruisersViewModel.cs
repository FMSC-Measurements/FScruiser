using FScruiser.Services;
using Prism.Navigation;
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

        public bool EnableCruiserPopup
        {
            get { return DataService.EnableCruiserPopup; }
            set { DataService.EnableCruiserPopup = value; }
        }

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

        protected override void Refresh(INavigationParameters parameters)
        {
            //do nothing
        }
    }
}