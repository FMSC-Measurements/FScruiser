# AppCenter 


# environment variables
The FScruiser build process makes use of environment variables on the developers machine in inject constants into the source code. Using Scripty, the script file FScruiser.Xamarin/Secrets.local.csx will run prior to the build generating the Secrets.local.cs file. Secrets.local.csx copies the environment variables `fscruiser_appcenter_key_droid`, `fscruiser_appcenter_key_uwp`, and `fscruiser_appcenter_key_ios`

