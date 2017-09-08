using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FScruiser.XF;
using System;
using System.Linq;
using Xamarin.Forms.Platform.Android;

namespace FScruiser.Droid
{
    [Activity(Label = "FScruiser", Icon = "@drawable/fscruiser_32dp", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            FreshMvvm.FreshIOC.Container.Register<ICruiseFolderService, CruiseFolderService>();

            var app = new App();

            LoadApplication(app);
        }
    }
}