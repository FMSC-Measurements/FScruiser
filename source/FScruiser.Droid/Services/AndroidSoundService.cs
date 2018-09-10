
using Android.Content;
using Android.Media;
using FScruiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FScruiser.Droid.Services
{
    public class AndroidSoundService : ISoundService
    {
        SoundPool _soundPool;
        private readonly int _tally;
        private readonly int _measure;
        private readonly int _insurance;

        public AndroidSoundService(Context context)
        {
            var assetManager = context.Resources.Assets;

            var values = assetManager.List("sounds");


            _soundPool = new SoundPool(5, Stream.Notification, 0);

            _tally = _soundPool.Load(assetManager.OpenFd("sounds/tally.wav"), 1);
            _measure = _soundPool.Load(assetManager.OpenFd("sounds/measure.wav"), 1);
            _insurance = _soundPool.Load(assetManager.OpenFd("sounds/insurance.wav"), 1);

        }

        public void SignalInsuranceTree()
        {
            _soundPool.Play(_insurance, 1.0f, 1.0f, 0, 0, 1.0f);
        }

        public void SignalInvalidAction()
        {
        }

        public void SignalMeasureTree()
        {
            _soundPool.Play(_measure, 1.0f, 1.0f, 0, 0, 1.0f);
        }

        public void SignalTally(bool force)
        {
            _soundPool.Play(_tally, 1.0f, 1.0f, 0, 0, 1.0f);
        }
    }
}
