using FScruiser.Droid.Services;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Droid
{
    public class AndroidServiceService : ServiceService
    {
        public AndroidServiceService()
        {
            CruiseFileService = new CruiseFileService();
            SoundService      = new AndroidSoundService();
        }
    }
}
