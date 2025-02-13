# SavannahManager
[![Build Status](https://dev.azure.com/AonaSuzutsuki/SavannahManager/_apis/build/status/AonaSuzutsuki.SavannahManager?branchName=master)](https://dev.azure.com/AonaSuzutsuki/SavannahManager/_build/latest?definitionId=3&branchName=master)  

Savannah Manager is a management tool for 7 Days to Die server.
The goal is to make all management possible with software. 

I developed it thinking that it will be easier even a little.  
As a background to development, my server administrator of acquaintance seemed to be bothersome managing server on cmd, so I thought it would be easier a little as I developed it.  
Also, it was troublesome for me to open several screens and I wanted to realize it with one screen or one software.  
One reason is that there were few domestic tools.  

The name was from the acquaintance and he said  "Ease management ('Saboreru' on Japanese )".  
I thought it was exactly so I used it.  
Thank you very much.  

# Require to run
1. Windows 7 SP1 or later
2. [.NET 8 Desktop Runtime or Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

# Development environment
1. Visual Studio 2022
2. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

# Build
## Visual Studio 2022
### Steps
1. Open SavannahManager.sln.
3. Click Build on Menu.

## MSBuild (without Visual Studio 2022)
### Required Tools
1. [Build Tools for Visual Studio 2022](https://www.visualstudio.com/ja/downloads/)
    1. .Net SDK

### Steps
1. Open the project folder at Command Prompt.
```sh
cd /D D:\Develop\Git\SavannahManager
```

2. Build with dotnet.
```ps
dotnet build -c Release
```

3. Move builded files.
Move all assemblies to "bin" directory on same directory of SavannahManager.sln.  
Execute build.ps with Command Prompt.  
```ps
powershell -NoProfile -ExecutionPolicy Unrestricted .\build.ps1
```

# Special Thanks
ReactiveProperty:               Copyright (c) 2018 neuecc, xin9le, okazuki  
Prism.Core:                     Copyright (c) .NET Foundation  
Microsoft.Xaml.Behaviors.Wpf:   Copyright (c) 2015 Microsoft  
NUnit:                          Copyright (c) 2019 Charlie Poole, Rob Prouse  
Newtonsoft.Json:                Copyright (c) 2007 James Newton-King  
Moq:                            Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD  
OpenCover:                      Copyright (c) 2011-2019 Shaun Wilde  
SSH.NET:                        Copyright (C) Renci 2010-2021

This software includes the work that is distributed in the Apache License 2.0
