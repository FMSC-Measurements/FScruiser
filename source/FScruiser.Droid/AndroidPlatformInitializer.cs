﻿using Android.App;
using FScruiser.Droid.Services;
using FScruiser.Services;
using Prism.Ioc;
using System;

namespace FScruiser.Droid
{
    public class AndroidPlatformInitializer : Prism.IPlatformInitializer
    {
        public AndroidPlatformInitializer(Activity hostActivity)
        {
            HostActivity = hostActivity ?? throw new ArgumentNullException(nameof(hostActivity));
        }

        public Activity HostActivity { get; protected set; }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<ISoundService>(new AndroidSoundService(HostActivity.ApplicationContext));
        }
    }
}