|Branch |Status |
|---|---|
|Master | [![Build status](https://build.appcenter.ms/v0.1/apps/e3580fc9-71f6-4007-98a3-8645b86a684e/branches/master/badge)](https://appcenter.ms) |
|Development | [![Build status](https://build.appcenter.ms/v0.1/apps/e3580fc9-71f6-4007-98a3-8645b86a684e/branches/Development/badge)](https://appcenter.ms) | 

# External Projects
external projects i.e. CruiseDAL, FMSC.ORM, FMSC.Sampling, Sqlbuilder are managed using nuget packages. This is not ideal but is due to a bug in visual studio that I encountered that caused problems using muti-targeted projects where the project file was from a different solution or not in a sub-folder contained in the main solution path. 
To update one of these external projects, open the solution for the external project and use the Pack (rightClick->Pack) command to generate a .nupkg file for the project. The .nupkg will be generated in the bin/Release folder for the project. Copy the .nupkg to the `FScruiser/source/packages` directory. 
From the FScruiser solution goto manage nuget packages on the FScruiser.Core project and update the respective external project package. Make sure that `Package source` is set to `SolutionNugetRepository`. This is a special nuget package source configured in the `source/NuGet.config` that tells visual studio to look for nuget packages in the `source/packages` directory