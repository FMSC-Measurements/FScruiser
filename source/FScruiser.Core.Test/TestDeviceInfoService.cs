using FScruiser.Services;

namespace FScruiser.Core.Test
{
    public class TestDeviceInfoService : IDeviceInfoService
    {
        public string GetDeviceName()
        {
            return "testDeviceName";
        }

        public string GetUniqueDeviceID()
        {
            return "testDeviceID";
        }
    }
}