using FScruiser.Services;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using Plugin.FilePicker;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using Xamarin.Forms;

namespace FScruiser.XF.ViewModels
{
    public class NavigationListItem
    {
        public string Title { get; set; }

        public string NavigationPath { get; set; }

        public bool CanShow
        {
            get
            {
                return CanShowAction?.Invoke() ?? true;
            }
        }

        public Func<NavigationParameters> GetParamaters { get; set; }

        public Func<bool> CanShowAction { get; set; }
    }

    public class MainViewModel : ViewModelBase
    {
        private Command _selectFileCommand;
        private Command<NavigationListItem> _navigateCommand;

        public ICommand SelectFileCommand => _selectFileCommand ?? (_selectFileCommand = new Command(SelectFileAsync));

        public ICommand NavigateCommand => _navigateCommand ?? (_navigateCommand = new Command<NavigationListItem>(async (x) => await NavigateToAsync(x)));

        public bool CanShowCuttingUnits => !string.IsNullOrWhiteSpace(SelectedUnitCode);

        public bool CanShowTallies => !string.IsNullOrWhiteSpace(SelectedUnitCode);

        public bool CanShowTrees => !string.IsNullOrWhiteSpace(SelectedUnitCode);

        public IEnumerable<NavigationListItem> NavigationListItems { get; set; }

        public string CurrentFilePath
        {
            get
            {
                var filePath = DatastoreProvider.Cruise_Path;
                if(string.IsNullOrWhiteSpace(filePath))
                {
                    return "Open File";
                }
                else
                {
                    return Path.GetFileName(filePath);
                }

            }
        }

        public ICuttingUnitDatastoreProvider DatastoreProvider { get; }
        protected IDialogService DialogService { get; set; }

        public string SelectedUnitCode { get; protected set; }

        public MainViewModel(INavigationService navigationService
            , IDialogService dialogService
            , ICuttingUnitDatastoreProvider datastoreProvider) : base(navigationService)
        {
            DialogService = dialogService;
            DatastoreProvider = datastoreProvider;

            RefreshNavigation();
        }

        public void RefreshNavigation()
        {
            var datastore = DatastoreProvider.CuttingUnitDatastore;

            if (datastore != null && SelectedUnitCode != null)
            {

                NavigationListItems = new NavigationListItem[]
                    {
                        new NavigationListItem {Title = "Cutting Units"
                        , NavigationPath = "Navigation/CuttingUnits"
                        , CanShowAction = () => CanShowCuttingUnits},

                        new NavigationListItem {Title = "Tally"
                        , NavigationPath = "Navigation/Tally"
                        , GetParamaters = () => new NavigationParameters($"UnitCode={SelectedUnitCode}")
                        , CanShowAction = () => CanShowTallies },

                        new NavigationListItem {Title="Trees"
                        , NavigationPath = "Navigation/Trees"
                        , GetParamaters = () => new NavigationParameters($"UnitCode={SelectedUnitCode}")
                        , CanShowAction = () => CanShowTrees },

                        new NavigationListItem {Title="Plots"
                        , NavigationPath = "Navigation/Plots"
                        , GetParamaters = () => new NavigationParameters($"UnitCode={SelectedUnitCode}")},

                        new NavigationListItem {Title="Cruisers"
                        , NavigationPath = "Navigation/Cruisers"}
                    };
            }
            else
            {
                NavigationListItems = new NavigationListItem[]
                    {
                        new NavigationListItem {Title = "Cutting Units"
                        , NavigationPath = "Navigation/CuttingUnits"},

                        new NavigationListItem {Title="Cruisers"
                        , NavigationPath = "Navigation/Cruisers"}
                    };
            }


        }

        public async System.Threading.Tasks.Task NavigateToAsync(NavigationListItem obj)
        {
            var navParams = obj?.GetParamaters?.Invoke();

            try
            {
                await NavigationService.NavigateAsync(obj.NavigationPath, navParams);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("ERROR::::" + ex);
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "nav_path", obj.NavigationPath } });
            }
        }

        private async void SelectFileAsync(object obj)
        {
            try
            {
                var fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null) { return; }//user canceled file picking

                var filePath = fileData.FilePath;

                //Check path exists
                if (File.Exists(filePath) == false)
                {
                    await DialogService.ShowMessageAsync(filePath, "File Doesn't Exist").ConfigureAwait(false);
                    return;
                }

                MessagingCenter.Send<object, string>(this, Messages.CRUISE_FILE_SELECTED, filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error:::{ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                Crashes.TrackError(ex);
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            MessagingCenter.Unsubscribe<Object>(this, Messages.CRUISE_FILE_OPENED);
            MessagingCenter.Unsubscribe<Object>(this, Messages.CRUISE_FILE_SELECTED);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            MessagingCenter.Subscribe<object, string>(this, Messages.CRUISE_FILE_OPENED, (sender, path) =>
            {
                SelectedUnitCode = null;
                RaisePropertyChanged(nameof(this.CurrentFilePath));
                RaisePropertyChanged(nameof(NavigationListItems));
            });
            MessagingCenter.Subscribe<string>(this, Messages.CUTTING_UNIT_SELECTED, (unit) =>
            {
                SelectedUnitCode = unit;
                RefreshNavigation();

                RaisePropertyChanged(nameof(NavigationListItems));
            });
        }
    }
}