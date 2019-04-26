# FScruiser.Core

## CruiseDAL.Core
	
## FMSC.Sampling

# FScruiser.Xamarin

## Xamarin.Forms
 currently using version 4.0-preview because it includes the new CollectionView. using the CollectionView in preview requires setting a flag in FScruiser.Droid.MainActivity.OnCreate

## FlowListView
Listview used for stratum selection for its ability to arrange items dynamicly in rows

## Microsoft.AppCenter
### Analytics 
for recieving user feed back, tracking feature useage, and proactive error tracking. 

### Crashes
for crash reports and logging exceptions 

### Distribute
for hooking up automatic update from AppCenter when using AppCenter for distributing. **Remove this dependancy when distributing to Play Store**

## Newtonsoft.Json
for serializing cruisers in `TallySettingsDataService.cs`

## Plugin.Permissions
for checking and requesting permissions if disabled. Depends on Plugin.CurrentActivity on Android

## Prism
MVVM framework, Navigation 

### Prism.Autofac.Forms
 variant of prism library that uses autofac under the covers for IOC. There is no direct dependency on autofac and other variants could be used without requiring code changes. 

## Xamarin.Plugin.FilePicker
for FilePicker
multiple forks of this library exist some better supported than others. Currently this seems the best supported Keep track of developments on this library. 

## Xamarin.Toolkit.*
useful suit of libraries that extend functionality of controls using Behaviors, Converters, and Effects

## Scripty.MsBuild
build tool. **not a run-time dependency **

# FScruiser.Droid

## Plugin.CurrentActivity 
required by Plugin.Permissions. Initialized in MainApplication.cs