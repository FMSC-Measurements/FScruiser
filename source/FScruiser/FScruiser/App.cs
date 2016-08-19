using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using FScruiser.ViewModels;
using FScruiser.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace FScruiser
{
    public class App : FormsApplication
    {
        readonly SimpleContainer _container;

        public App(SimpleContainer container)
        {
            _container = container;

            _container.PerRequest<MainPageViewModel>();

            Initialize();

            DisplayRootView<MainPage>();
        }

        protected override void PrepareViewFirst(NavigationPage navigationPage)
        {
            _container.Instance<INavigationService>(new NavigationPageAdapter(navigationPage));
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