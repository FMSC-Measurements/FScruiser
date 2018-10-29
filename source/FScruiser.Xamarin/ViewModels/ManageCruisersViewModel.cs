using FScruiser.XF.Services;
using Prism.Navigation;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class ManageCruisersViewModel : ViewModelBase
    {
        private Command<string> _addCruiserCommand;
        private Command<string> _removeCruiserCommand;

        public TallySettingsDataService Data { get; }

        public ICommand AddCruiserCommand => _addCruiserCommand ?? (_addCruiserCommand = new Command<string>(AddCruiser));

        public ICommand RemoveCruiserCommand => _removeCruiserCommand ?? (_removeCruiserCommand = new Command<string>(RemoveCruiser));

        public ManageCruisersViewModel(INavigationService navigationService) : base(navigationService)
        {
            Data = new TallySettingsDataService();
        }

        public void AddCruiser(string cruiser)
        {
            Data.AddCruiser(cruiser);

            Data.Save();
        }

        public void RemoveCruiser(string cruiser)
        {
            Data.RemoveCruiser(cruiser);

            Data.Save();
        }

        protected override void Refresh(INavigationParameters parameters)
        {
            Data.Refresh();
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            Data.Save();
        }
    }
}