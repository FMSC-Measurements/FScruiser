using FScruiser.Pages;
using FScruiser.Services;
using FScruiser.XF.Services;
using FScruiser.XF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace FScruiser.XF
{
    public class App : Xamarin.Forms.Application
    {
        public static ServiceService ServiceService { get; set; }

        //public static bool InDesignMode = true;
//#if DEBUG
//        = true;
//#else
//        = false;
//#endif


        public App(ServiceService serviceService)
        {
            serviceService.DialogService            = new XamarinDialogService();
            serviceService.TallySettingsDataService = new TallySettingsDataService();

            ServiceService = serviceService;

            


            var view = new MainPage();
            var viewModel = new MainViewModel(view.Navigation);
            view.BindingContext = viewModel;
            viewModel.Init();

            MainPage = new NavigationPage(view);

            //MainPage = new FreshMvvm.FreshNavigationContainer(FreshMvvm.FreshPageModelResolver.ResolvePageModel<MainViewModel>());
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