# CLI-To-Do
[![Build, Test and Release](https://github.com/oikz/CLI-To-Do/actions/workflows/dotnet.yml/badge.svg)](https://github.com/oikz/CLI-To-Do/actions/workflows/dotnet.yml)  

## What is it?
CLI-To-Do is a command line tool to help you create new Tasks and Reminders using a simple and quick Command line interface.  

#### Currently supported reminder platforms:
- [Microsoft To Do](https://todo.microsoft.com/)
- Google Tasks
  - Authorization will show that this application is not verified as Google does not verify applications that don't have a specific visual Google sign in button (not achievable within a terminal).
  - Setting the due time of the reminder is not supported by Google Tasks so is not available.  
## Usage
- Clone this repository
- Run with `dotnet run --project "CLI To Do"` 
- Or publish to an executable using `dotnet publish -r [OS] -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true` replacing "[OS]" with a suitable value from [here](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)   
e.g., `win-x64`, `osx-x64` or `linux-x64`
- Run the executable
- To switch between reminder platforms after choosing, delete the file "platformConfig.txt" located within the todo folder inside your user folder.

## Date Shortcuts
- "tomorrow"
- Weekdays (e.g. "monday")
- "" for today