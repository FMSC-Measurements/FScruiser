using FScruiser.Services;
using FScruiser.XF.Pages;
using FScruiser.XF.Services;
using Xamarin.Forms.Xaml;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

namespace FScruiser.XF
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Xamarin.Forms.Application
    {
        public static ServiceService ServiceService { get; set; }

        //public static bool InDesignMode = true;
        //#if DEBUG
        //        = true;
        //#else
        //        = false;
        //#endif

        public App()
        {
            this.InitializeComponent();
        }

        public App(ServiceService serviceService) : this()
        {
            
            serviceService.DialogService = new XamarinDialogService(serviceService);
            serviceService.TallySettingsDataService = new TallySettingsDataService();

            ServiceService = serviceService;

            var view = new UnitTallyMasterDetailPage();

            MainPage = view;
        }

        protected override void OnStart()
        {
            // Handle when your app starts

            //start app center services
//#if !DEBUG
            AppCenter.Start($"ios={Secrets.APPCENTER_KEY_IOS};android={Secrets.APPCENTER_KEY_DROID};uwp={Secrets.APPCENTER_KEY_UWP}", typeof(Distribute), typeof(Analytics), typeof(Crashes));
//#endif
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}