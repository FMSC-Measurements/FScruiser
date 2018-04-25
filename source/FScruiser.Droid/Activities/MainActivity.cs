using Android.App;
using Android.Content.PM;
using Android.OS;
using FScruiser.XF;
using Xamarin.Forms.Platform.Android;

namespace FScruiser.Droid
{
    [Activity(Label = "FScruiser", Icon = "@drawable/fscruiser_32dp", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            var app = new App(new AndroidServiceService());

            LoadApplication(app);
        }
    }
}