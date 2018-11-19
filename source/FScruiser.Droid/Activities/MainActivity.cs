using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using FScruiser.XF;
using Plugin.Permissions;
using Xamarin.Forms.Platform.Android;

namespace FScruiser.Droid
{
    [Activity(Label = "FScruiser", Icon = "@drawable/fscruiser_32dp", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.MyTheme);//set theme to main theme, because it should be set at launch to the splash theme

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Toolkit.Effects.Droid.Effects.Init();
            DLToolkit.Forms.Controls.FlowListView.Init();

            var app = new App(new AndroidPlatformInitializer(this));

            LoadApplication(app);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}