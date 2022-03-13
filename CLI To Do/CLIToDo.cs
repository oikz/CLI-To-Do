﻿using System;
using System.IO;
using System.Linq;
using CLI_To_Do.GoogleTasks;
using CLI_To_Do.MicrosoftToDo;
using Google.Apis.Auth.OAuth2;
using Microsoft.Graph;
using Google.Apis.Tasks.v1;
using Task = System.Threading.Tasks.Task;

namespace CLI_To_Do;

public static class CLIToDo {
    private const string ToDoAppId = "8c6a9efb-30e2-4c95-b975-9a46a82cfaf0"; //Spooky appID
    private const string GoogleClientId = "82771178022-gba3dto0pftqk8n9nd6lell0ko8jrler.apps.googleusercontent.com";
    private const string GoogleClientSecret = "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";
    private static readonly string[] ToDoScopes = { "User.Read", "Tasks.Read", "Tasks.ReadWrite" };
    private static readonly string[] GoogleScopes = { TasksService.Scope.Tasks };
    
    public static async Task Main(string[] args) {
        System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\todo\\");
        await ChoosePlatform();
    }

    private static async Task ChoosePlatform() {
        Console.WriteLine("Welcome to CLI To Do");
        Console.WriteLine("Please choose a platform to use: ");
        Console.WriteLine("1. Microsoft To Do");
        Console.WriteLine("2. Google Tasks");
        Console.WriteLine("3. Exit");
        var input = UserInterface.ChoosePlatform();
        switch (input) {
            case 1:
                await MicrosoftToDo();
                break;
            case 2:
                await GoogleTasks();
                break;
            case 3:
                Environment.Exit(0);
                break;
        }
    }

    private static async Task GoogleTasks() {
        var credentials = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\todo\\credentials.json";
        TasksService service;
        if (System.IO.File.Exists(credentials)) {
            await using var stream = new FileStream(credentials, FileMode.Open, FileAccess.Read);
            var secrets = (await GoogleClientSecrets.FromStreamAsync(stream)).Secrets;
            service = GoogleTaskHelper.CreateService(secrets.ClientId, secrets.ClientSecret, GoogleScopes);
        } else {
            service = GoogleTaskHelper.CreateService(GoogleClientId, GoogleClientSecret, GoogleScopes);
        }

        var task = new Google.Apis.Tasks.v1.Data.Task {
            Title = UserInterface.GetTitle()
        };

        var lists = GoogleTaskHelper.GetLists(service);
        var listID = lists.ElementAt(UserInterface.GetListsHelper(lists.Count) - 1).Id; //Get the chosen list
        
        //Setup the date and time for the task/reminder
        Console.WriteLine("\nDate: Format: YYYY-MM-DD (Empty for today) ");
        var dateString = UserInterface.GetDate() + "T00:00:00.000Z";
        task.Due = dateString;

        await service.Tasks.Insert(task, listID).ExecuteAsync();
    }

    private static async Task MicrosoftToDo() {
        var authProvider = new DeviceCodeAuthProvider(ToDoAppId, ToDoScopes);

        // Initialize Graph client
        ToDoTaskHelper.Initialize(authProvider);

        // Get signed in user
        var user = ToDoTaskHelper.GetMeAsync().Result;
        Console.WriteLine($"Welcome {user.DisplayName}!");

        //make new task because it doesnt work unless i do this or smth
        var newTask = new TodoTask { ODataType = null, Title = UserInterface.GetTitle() };

        //ID for the list to add the task to
        var lists = await ToDoTaskHelper.GetLists();
        var listID = lists.ElementAt(UserInterface.GetListsHelper(lists.Count) - 1).Id; //Get the chosen list

        //Setup the date and time for the task/reminder
        Console.WriteLine("\nDate: Format: YYYY-MM-DD (Empty for today) ");
        var dateString = UserInterface.GetDate();

        Console.Write("\nTime: (Empty for no reminder) ");
        var newTime = UserInterface.GetTime();

        ToDoTaskHelper.SetDates(dateString, newTime, newTask);

        await ToDoTaskHelper.CreateTask(newTask, listID);
    }
}