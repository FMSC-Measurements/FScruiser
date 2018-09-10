using FScruiser.Services;
using FScruiser.Util;
using FScruiser.XF.Pages;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Prism.Autofac.PrismApplication
    {
        public const string CURRENT_NAV_PATH = "current_nav_path";
        public const string CURRENT_NAV_PARAMS = "current_nav_params";

        public ICuttingUnitDatastoreProvider CuttingUnitDatastoresProvider { get; private set; } = new CuttingUnitDatastoreProvider();

        private App() { }

        public App(IPlatformInitializer platformInitializer) : base(platformInitializer)
        { }

        protected override async void OnInitialized()
        {
            this.InitializeComponent();

#if RELEASE
            //start app center services
            AppCenter.Start($"ios={Secrets.APPCENTER_KEY_IOS};android={Secrets.APPCENTER_KEY_DROID};uwp={Secrets.APPCENTER_KEY_UWP}", typeof(Distribute), typeof(Analytics), typeof(Crashes));
#endif

            MessagingCenter.Subscribe<object, string>(this, Messages.CRUISE_FILE_SELECTED, (sender, path) =>
            {
                LoadCruiseFile(path);
            });

            MessagingCenter.Subscribe<object, string>(this, Messages.PAGE_NAVIGATED_TO, (sender, navParams) =>
            {
                try
                {
                    var navigationPath = NavigationService.GetNavigationUriPath();

                    Properties.SetValue(CURRENT_NAV_PATH, navigationPath);
                    Properties.SetValue(CURRENT_NAV_PARAMS, navParams);
                }catch( Exception ex)
                {
                    Debug.WriteLine("ERROR::::" + ex);
                    Crashes.TrackError(ex);
                }
            });

            try
            {
                await NavigationService.NavigateAsync("/Main/Navigation/CuttingUnits");
            }
            catch(Exception ex)
            {
                Debug.WriteLine("ERROR::::" + ex);
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "nav_path", "/Main/Navigation/CuttingUnits" } });
            }
        }

        protected void LoadCruiseFile(string path)
        {
            //var cruiseFileType = CruiseDAL.DAL.ExtrapolateCruiseFileType(path);
            //if (cruiseFileType.HasFlag(CruiseDAL.CruiseFileType.Cruise))
            //{
            //    Analytics.TrackEvent("Error::::LoadCruiseFile|Invalid File Path", new Dictionary<string, string>() { { "FilePath", path } });
            //    return;
            //}

            try
            {
                var datastore = new CuttingUnitDatastore(path);

                CuttingUnitDatastoresProvider.CuttingUnitDatastore = datastore;
                CuttingUnitDatastoresProvider.SampleSelectorDataService = new SampleSelectorRepository(datastore);

                //((IContainerRegistry)Container).RegisterInstance<ICuttingUnitDatastore>(datastore);
                //((IContainerRegistry)Container).RegisterInstance<ISampleSelectorDataService>(new SampleSelectorRepository(datastore));

                //var test = Container.Resolve<ICuttingUnitDatastore>();

                Properties.SetValue("cruise_path", path);
                CuttingUnitDatastoresProvider.Cruise_Path = path;
                NavigationService.NavigateAsync("/Main/Navigation/CuttingUnits");

                MessagingCenter.Send<object, string>(this, Messages.CRUISE_FILE_OPENED, path);
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine($"Error:::{ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                Crashes.TrackError(ex, new Dictionary<string, string> { { "FilePath", path } });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error:::{ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                Crashes.TrackError(ex, new Dictionary<string, string> { { "FilePath", path } });

                var dialogSerive = Container.Resolve<IDialogService>();

                dialogSerive?.ShowMessageAsync(ex.ToString(), "File Could Not Be Opended").ConfigureAwait(false);
            }
        }

        protected void ReloadNavigation()
        {
            var navPath = Properties.GetValueOrDefault(CURRENT_NAV_PATH) as string;

            if(navPath != null && !navPath.EndsWith("CuttingUnits"))
            {
                var navParams = Properties.GetValueOrDefault(CURRENT_NAV_PARAMS) as string;

                NavigationService.NavigateAsync(navPath, new NavigationParameters(navParams));
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts

            var cruise_path = Properties.GetValueOrDefault("cruise_path") as string;

            if (!string.IsNullOrEmpty(cruise_path))
            {
                LoadCruiseFile(cruise_path);
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDialogService, XamarinDialogService>();
            containerRegistry.RegisterSingleton<ITallySettingsDataService, TallySettingsDataService>();
            //containerRegistry.RegisterInstance<ICuttingUnitDatastore>(null);

            containerRegistry.RegisterInstance<ICuttingUnitDatastoreProvider>(this.CuttingUnitDatastoresProvider);

            containerRegistry.RegisterForNavigation<MyNavigationPage>("Navigation");
            containerRegistry.RegisterForNavigation<MainPage, ViewModels.MainViewModel>("Main");
            containerRegistry.RegisterForNavigation<CuttingUnitListPage, ViewModels.CuttingUnitListViewModel>("CuttingUnits");
            containerRegistry.RegisterForNavigation<UnitTreeTallyPage, ViewModels.UnitTreeTallyViewModel>("Tally");
            containerRegistry.RegisterForNavigation<TreeListPage, ViewModels.TreeListViewModel>("Trees");
            containerRegistry.RegisterForNavigation<TreeEditPage2, ViewModels.TreeEditViewModel>("Tree");
            containerRegistry.RegisterForNavigation<Pages.PlotListPage, ViewModels.PlotListViewModel>("Plots");
            containerRegistry.RegisterForNavigation<Pages.PlotTallyPage, ViewModels.PlotTallyViewModel>("PlotTally");
            containerRegistry.RegisterForNavigation<Pages.PlotEditPage, ViewModels.PlotEditViewModel>("PlotEdit");
            containerRegistry.RegisterForNavigation<Pages.ManageCruisersPage, ViewModels.ManagerCruisersViewModel>("Cruisers");
        }
    }
}