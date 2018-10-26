using FScruiser.Services;
using Prism.Ioc;
using System;

namespace FScruiser.XF
{
    public class TestPlatformInitializer : Prism.IPlatformInitializer
    {
        //private ISoundService SoundService { get; }

        public TestPlatformInitializer()
        { }

        //public TestPlatformInitializer(ISoundService soundService) : this()
        //{
        //    SoundService = soundService ?? throw new ArgumentNullException(nameof(soundService));
        //}

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //if (SoundService != null)
            //{
            //    containerRegistry.RegisterInstance<ISoundService>(SoundService);
            //}
        }
    }
}