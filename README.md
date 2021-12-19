# CLI-To-Do
[![build and test](https://github.com/oikz/CLI-To-Do/actions/workflows/dotnet.yml/badge.svg?branch=Github-Actions)](https://github.com/oikz/CLI-To-Do/actions/workflows/dotnet.yml)  
Proof of concept command line based implementation of Microsoft To Do using the Microsoft Graph API

Made this because I hated how long it takes to open the app and then create a reminder  
Also wanted to learn C# a bit 

Currently can only login and add tasks (The only features I planned on using)

## Build  
Built using:  
`dotnet publish -r [OS] -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true`  
replacing "[OS]" with win-x64, osx-x64 or linux-x64 (untested)
