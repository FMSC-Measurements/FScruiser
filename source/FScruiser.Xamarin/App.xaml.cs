using CruiseDAL;
using FScruiser.Services;
using FScruiser.Util;
using FScruiser.XF.Pages;
using FScruiser.XF.Services;
using Microsoft.AppCenter.Crashes;
using Plugin.Permissions;
using Prism;
using Prism.Ioc;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FScruiser.XF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Prism.Autofac.PrismApplication
    {
        public const string CURRENT_NAV_PATH = "current_nav_path";
        public const string CURRENT_NAV_PARAMS = "current_nav_params";

        protected IPageDialogService DialogService => Container?.Resolve<IPageDialogService>();

        public ICuttingUnitDatastoreProvider CuttingUnitDatastoresProvider { get; private set; } = new CuttingUnitDatastoreProvider();

        public IApplicationSettings Settings { get; } = new ApplicationSettings();

        public App()
        {
        }

        public App(IPlatformInitializer platformInitializer) : base(platformInitializer)
        { }

        protected override async void OnInitialized()
        {
            this.InitializeComponent();

#if RELEASE
            //start app center services
            Microsoft.AppCenter.AppCenter.Start($"ios={Secrets.APPCENTER_KEY_IOS};android={Secrets.APPCENTER_KEY_DROID};uwp={Secrets.APPCENTER_KEY_UWP}"
                , typeof(Microsoft.AppCenter.Distribute.Distribute)
                , typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Crashes));

            
#endif
            

            MessagingCenter.Subscribe<object, string>(this, Messages.CRUISE_FILE_SELECTED, async (sender, path) =>
            {
                await LoadCruiseFileAsync(path);
            });

            //MessagingCenter.Subscribe<object, string>(this, Messages.PAGE_NAVIGATED_TO, (sender, navParams) =>
            //{
            //    try
            //    {
            //        var navigationPath = NavigationService.GetNavigationUriPath();

            //        Properties.SetValue(CURRENT_NAV_PATH, navigationPath);
            //        Properties.SetValue(CURRENT_NAV_PARAMS, navParams);
            //    }catch( Exception ex)
            //    {
            //        Debug.WriteLine("ERROR::::" + ex);
            //        Crashes.TrackError(ex);
            //    }
            //});

            await NavigationService.NavigateAsync("/Main/Navigation/CuttingUnits");
        }

        protected async System.Threading.Tasks.Task LoadCruiseFileAsync(string path)
        {
            //var cruiseFileType = CruiseDAL.DAL.ExtrapolateCruiseFileType(path);
            //if (cruiseFileType.HasFlag(CruiseDAL.CruiseFileType.Cruise))
            //{
            //    Analytics.TrackEvent("Error::::LoadCruiseFile|Invalid File Path", new Dictionary<string, string>() { { "FilePath", path } });
            //    return;
            //}

            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);

                if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions.Abstractions.Permission.Storage))
                    {
                        await DialogService.DisplayAlertAsync("Request Access To Storage", "FScruiser needs access to files on device", "OK");
                    }

                    var requestResults = await CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                    status = requestResults.GetValueOrDefault(Plugin.Permissions.Abstractions.Permission.Storage);
                }

                if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    try
                    {
                        var database = new DAL(path);

                        if (CruiseDAL.Updater.CheckNeedsMajorUpdate(database))
                        {
                            var result = await DialogService.DisplayAlertAsync("Update Cruise File?",
                                "Cruise file needs to be updated.\r\n" +
                                "Onece updated file can only be opened with latest cruise software",
                                "Update", "Cancel");
                            if (result)
                            {
                                CruiseDAL.Updater.UpdateMajorVersion(database);
                            }
                            else
                            {
                                return;
                            }
                        }

                        var datastore = new CuttingUnitDatastore(database);

                        CuttingUnitDatastoresProvider.CuttingUnitDatastore = datastore;
                        CuttingUnitDatastoresProvider.SampleSelectorDataService = new SampleSelectorRepository(datastore);

                        //((IContainerRegistry)Container).RegisterInstance<ICuttingUnitDatastore>(datastore);
                        //((IContainerRegistry)Container).RegisterInstance<ISampleSelectorDataService>(new SampleSelectorRepository(datastore));

                        //var test = Container.Resolve<ICuttingUnitDatastore>();

                        Properties.SetValue("cruise_path", path);
                        CuttingUnitDatastoresProvider.Cruise_Path = path;
                        await NavigationService.NavigateAsync("/Main/Navigation/CuttingUnits");

                        MessagingCenter.Send<object, string>(this, Messages.CRUISE_FILE_OPENED, path);
                    }
                    catch (FileNotFoundException ex)
                    {
                        var task = LogAndShowExceptionAsync("File Error", "File Not Found", ex, new Dictionary<string, string> { { "FilePath", path } });
                    }
                    catch (Exception ex)
                    {
                        var task = LogAndShowExceptionAsync("File Error", "File Could Not Be Opended", ex, new Dictionary<string, string> { { "FilePath", path } });
                    }
                }
            }
            catch (Exception ex)
            {
                LogException("permissions", "request permissions error in method LoadCruiseFileAsync", ex);
            }
        }

        //protected void ReloadNavigation()
        //{
        //    var navPath = Properties.GetValueOrDefault(CURRENT_NAV_PATH) as string;

        //    if(navPath != null && !navPath.EndsWith("CuttingUnits"))
        //    {
        //        var navParams = Properties.GetValueOrDefault(CURRENT_NAV_PARAMS) as string;

        //        NavigationService.NavigateAsync(navPath, new NavigationParameters(navParams));
        //    }
        //}

        protected override async void OnStart()
        {
            // Handle when your app starts

            var isFirstRun = Properties.GetValueOrDefault<bool>("isFirstLaunch", true);
            if (isFirstRun)
            {
                var enableDiagnostics = await DialogService.DisplayAlertAsync("", $"FScruiser can automaticly send dianostics and crash reports.{Environment.NewLine}{Environment.NewLine} " +
                    $"This feature is optional, however, it helps us to create a better experience.{Environment.NewLine}{Environment.NewLine}" +
                    $"Would you like to enable this feature?{Environment.NewLine}{Environment.NewLine}" +
                    "You can also change this option in the settings page.", "Enable", "No Thanks");

                Settings.EnableAnalitics = Settings.EnableCrashReports = enableDiagnostics;

                Properties.SetValue("isFirstLaunch", false);
            }

            var cruise_path = Properties.GetValueOrDefault("cruise_path") as string;

            if (!string.IsNullOrEmpty(cruise_path))
            {
                await LoadCruiseFileAsync(cruise_path);
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
            containerRegistry.RegisterForNavigation<Pages.ThreePPNTPlotPage, ViewModels.ThreePPNTPlotViewModel>("ThreePPNTPlot");
            containerRegistry.RegisterForNavigation<Pages.ManageCruisersPage, ViewModels.ManageCruisersViewModel>("Cruisers");
            containerRegistry.RegisterForNavigation<Pages.SettingsPage, ViewModels.SettingsViewModel>("Settings");
            containerRegistry.RegisterForNavigation<Pages.LimitingDistancePage, ViewModels.LimitingDistanceViewModel>("LimitingDistance");
            containerRegistry.RegisterForNavigation<Pages.LogsListPage, ViewModels.LogsListViewModel>("Logs");
            containerRegistry.RegisterForNavigation<Pages.LogEditPage, ViewModels.LogEditViewModel>("Log");
            containerRegistry.RegisterForNavigation<Pages.FeedbackPage>("Feedback");
        }

        public static void LogException(string catigory, string message, Exception ex, IDictionary<string, string> data = null)
        {
            Debug.WriteLine($"Error:::{catigory}::::{message}::::{ex.Message}::::");
            Debug.WriteLine(ex.StackTrace);

            if (data == null) { data = new Dictionary<string, string>(); }

            data.SetValue("error_catigory", catigory);
            data.SetValue("error_message", message);

            Crashes.TrackError(ex, data);
        }

        public async Task LogAndShowExceptionAsync(string catigory, string message, Exception ex, IDictionary<string, string> data = null)
        {
            LogException(catigory, message, ex, data);

            var result = await DialogService.DisplayAlertAsync(catigory, message, "Details", "OK");
            if (result == true)//user clicked Details
            {
                await DialogService.DisplayAlertAsync(catigory, ex.ToString(), "Close");
            }
        }
    }
}