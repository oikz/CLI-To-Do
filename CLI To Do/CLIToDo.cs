﻿using System;
using Microsoft.Graph;
using System.Threading.Tasks;

namespace CLI_To_Do;

public static class CLIToDo {
    //Global Variable Goodies
    private const string AppId = "8c6a9efb-30e2-4c95-b975-9a46a82cfaf0"; //Spooky appID
    private static readonly string[] Scopes = { "User.Read", "Tasks.Read", "Tasks.ReadWrite" };


    public static async Task Main(string[] args) {
        await start();
    }

    //Start the program
    private static async Task start() {
        var authProvider = new DeviceCodeAuthProvider(AppId, Scopes);

        // Initialize Graph client
        TaskHelper.Initialize(authProvider);

        //Create new folder for storing data if not already created
        System.IO.Directory.CreateDirectory(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\todo\\");

        // Get signed in user
        var user = TaskHelper.GetMeAsync().Result;
        Console.WriteLine($"Welcome {user.DisplayName}!");

        //make new task because it doesnt work unless i do this or smth
        var newTask = new TodoTask { ODataType = null, Title = UserInterface.getTitle() };

        //Get title

        //ID for the list to add the task to
        var listID = TaskHelper.getLists().Result;

        //Setup the date and time for the task/reminder

        Console.WriteLine("Date: Format: YYYY-MM-DD (Empty for today)");
        var dateString = UserInterface.getDate();

        Console.Write("Time: ");
        var newTime = UserInterface.getTime();

        //Set all the juicy task info
        var reminderTime = new DateTimeTimeZone {
            ODataType = null, //Required for whatever reason
            TimeZone = TimeZoneInfo.Local.StandardName,
            DateTime = dateString + "T" + newTime.TimeOfDay + ".0000000"
        };

        newTask.ReminderDateTime = reminderTime;

        //No set time
        if (newTime.TimeOfDay.ToString() != "") {
            newTask.ReminderDateTime = reminderTime;
        }

        newTask.DueDateTime = reminderTime; //Set the due date for the reminder

        await TaskHelper.CreateTask(newTask, listID);
    }
}