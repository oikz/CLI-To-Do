using System;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

class CLIToDo {
    //Global Variable Goodies
    public static string tenantID = "fce5109a-b406-449c-8c1e-48319af5ab15";//Spooky tenantID

    static void Main(string[] args) {
        CLIToDo instance = new CLIToDo();
        instance.start();
    }

    //Start the program
    private void start() {
        var appConfig = LoadAppSettings();

        if (appConfig == null) {
            Console.WriteLine("Missing or invalid appsettings.json...exiting");
            return;
        }

        var appId = appConfig["appId"];
        var scopesString = appConfig["scopes"];
        var scopes = scopesString.Split(';');

        // Initialize the auth provider with values from appsettings.json
        var authProvider = new DeviceCodeAuthProvider(appId, scopes);

        // Request a token to sign in the user
        var accessToken = authProvider.GetAccessToken().Result;




        Console.WriteLine("CLI To Do\n");
        TodoTask newTask = new TodoTask();
        Console.WriteLine("Title: ");
        newTask.Title = Console.ReadLine();


        DateTime newTime;

        Console.WriteLine("Date: Format: YYYY-MM-DD (Empty for today)");
        string dateString = getDate();

        Console.WriteLine("Time: ");
        newTime = getTime();

        //Set all the juicy task info
        DateTimeTimeZone reminderTime = new DateTimeTimeZone();
        reminderTime.TimeZone = "Pacific/Auckland";
        reminderTime.DateTime = dateString + "T" + newTime.TimeOfDay.ToString();
        newTask.ReminderDateTime = reminderTime;
        newTask.DueDateTime = reminderTime;
        Console.WriteLine(newTask.ReminderDateTime.DateTime.ToString());

    }

    //Separate Methods for niceness
    static private string getDate() {
        string date = Console.ReadLine();
        if (date == "") {
            DateTime newDate;
            newDate = DateTime.Today;
            
            //Cursed
            return newDate.Year.ToString() + "-" + newDate.Month.ToString() + "-" + newDate.Day.ToString();
        }
        try {
            DateTime newDate = Convert.ToDateTime(date);
            return newDate.Year.ToString() + "-" + newDate.Month.ToString() + "-" + newDate.Day.ToString();
        }
        catch {
            Console.WriteLine("Try Again");
            return getDate();
        }
    }

    static private DateTime getTime() {
        string time = Console.ReadLine();
        DateTime newTime;
        try {
            newTime = Convert.ToDateTime(time);
            return newTime;
        }
        catch {
            Console.WriteLine("Try Again");
            return getTime();
        }
    }

    static IConfigurationRoot LoadAppSettings() {
        var appConfig = new ConfigurationBuilder()
            .AddUserSecrets<CLIToDo>()
            .Build();

        // Check for required settings
        if (string.IsNullOrEmpty(appConfig["appId"]) ||
            string.IsNullOrEmpty(appConfig["scopes"])) {
            return null;
        }

        return appConfig;
    }
}