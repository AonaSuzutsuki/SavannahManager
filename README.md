# SavannahManager
Savannah Manager is a management tool for 7 Days to Die server.
The goal is to make all management possible with software. 

I developed it thinking that it will be easier even a little.  
As a background to development, my server administrator of acquaintance seemed to be bothersome managing server on cmd, so I thought it would be easier a little as I developed it.  
Also, it was troublesome for me to open several screens and I wanted to realize it with one screen or one software.  
One reason is that there were few domestic tools.  

The name was from the acquaintance and he said  "Ease management ('Saboreru' on Japanese )".  
I thought it was exactly so I used it.  
Thank you very much.  

# Development environment
1. Visual Studio 2017
2. Microsoft .NET Framework 4.6

# Minimum configuration required
1. [Microsoft .NET Framework 4.6](https://msdn.microsoft.com/ja-jp/library/5a4x27ek(v=vs.110).aspx)
2. [Microsoft .NET Framework 4.6 targeting pack](http://www.microsoft.com/ja-jp/download/details.aspx?id=48136)

# Build
## Visual Studio 2017
### Steps
1. Open SavannahManager.sln.
2. Set a target to x86.
3. Click Build on Menu.

## MSBuild (without Visual Studio 2017)
### Required Tools
1. [Build Tools for Visual Studio 2017](https://www.visualstudio.com/ja/downloads/#build-tools-for-visual-studio-2017)

### Steps
1. Open the project folder at cmd.
```sh
cd /D D:\Develop\Git\SavannahManager
```
2. Set through MSBuild's path.
```sh
set PATH=C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin;%PATH%
```
3. Build with MSBuild.
```sh
msbuild SavannahManager.sln /t:Clean;Build /p:Configuration=Release /p:PlatformTarget=x86 /p:OutputPath=bin
```