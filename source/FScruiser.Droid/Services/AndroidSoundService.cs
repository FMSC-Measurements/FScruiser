
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

            
            var audioAttrs = new AudioAttributes.Builder()
                .SetContentType(AudioContentType.Sonification)
                .SetUsage(AudioUsageKind.AssistanceSonification)
                .Build();

            _soundPool = new SoundPool.Builder()
                .SetAudioAttributes(audioAttrs)
                .SetMaxStreams(5)
                .Build();

            _tally = _soundPool.Load(assetManager.OpenFd("sounds/tally.wav"), 1);
            _measure = _soundPool.Load(assetManager.OpenFd("sounds/measure.wav"), 1);
            _insurance = _soundPool.Load(assetManager.OpenFd("sounds/insurance.wav"), 1);

        }

        public Task SignalInsuranceTreeAsync()
        {
            return Task.Run(() => _soundPool.Play(_insurance, 1.0f, 1.0f, 0, 0, 1.0f));            
        }

        public Task SignalInvalidActionAsync()
        {
            return Task.CompletedTask;
        }

        public Task SignalMeasureTreeAsync()
        {
            return Task.Run(() => _soundPool.Play(_measure, 1.0f, 1.0f, 0, 0, 1.0f));
        }

        public Task SignalTallyAsync(bool force)
        {
            return Task.Run(() => _soundPool.Play(_tally, 1.0f, 1.0f, 0, 0, 1.0f));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _soundPool?.Release();
                    _soundPool = null;
                }

                

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
