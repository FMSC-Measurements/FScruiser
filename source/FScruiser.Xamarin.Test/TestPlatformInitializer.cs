using FScruiser.Services;
using Prism.Ioc;
using System;

namespace FScruiser.XF
{
    public class TestPlatformInitializer : Prism.IPlatformInitializer
    {
        private ISoundService SoundService { get; }

        public TestPlatformInitializer(ISoundService soundService)
        {
            SoundService = soundService ?? throw new ArgumentNullException(nameof(soundService));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<ISoundService>(SoundService);
        }
    }
}