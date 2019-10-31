using Android.App;
using Android.OS;
using FScruiser.Services;

namespace FScruiser.Droid.Services
{
    public class AndroidDeviceInfoService : IDeviceInfoService
    {
        public string GetDeviceName()
        {
            var name = Android.Provider.Settings.System.GetString(Application.Context.ContentResolver, "device_name");
            if (string.IsNullOrWhiteSpace(name))
                name = Build.Model;

            return name;
        }

        public string GetUniqueDeviceID()
        {
            return Build.Serial;
        }
    }
}