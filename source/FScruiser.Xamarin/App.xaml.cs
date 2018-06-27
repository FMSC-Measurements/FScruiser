﻿using FScruiser.Services;
using FScruiser.XF.Pages;
using FScruiser.XF.Services;
using Xamarin.Forms.Xaml;

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