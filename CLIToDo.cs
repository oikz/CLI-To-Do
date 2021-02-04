using System;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class CLIToDo {
    //Global Variable Goodies
    public static string tenantID = "fce5109a-b406-449c-8c1e-48319af5ab15"; //Spooky tenantID

    static async Task Main(string[] args) {
        CLIToDo instance = new CLIToDo();
        await instance.start();
    }

    //Start the program
    private async Task start() {
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

        // Initialize Graph client
        TaskHelper.Initialize(authProvider);

        // Get signed in user
        var user = TaskHelper.GetMeAsync().Result;
        Console.WriteLine($"Welcome {user.DisplayName}!");

        //make new task because it doesnt work unless i do this or smth
        var newTask = new TodoTask {
            ODataType = null
        };

        Console.Write("Title: ");
        newTask.Title = Console.ReadLine();


        DateTime newTime;

        Console.WriteLine("Date: Format: YYYY-MM-DD (Empty for today)");
        string dateString = getDate();

        Console.Write("Time: ");
        newTime = getTime();

        //Set all the juicy task info
        var reminderTime = new DateTimeTimeZone();
        reminderTime.ODataType = null; //Required for whatever reason
        reminderTime.TimeZone = "Pacific/Auckland";
        reminderTime.DateTime = dateString + "T" + newTime.TimeOfDay.ToString() + ".0000000";
        newTask.ReminderDateTime = reminderTime;

        //No set time
        if (newTime.TimeOfDay.ToString() != "") {
            newTask.ReminderDateTime = reminderTime;
        }

        newTask.DueDateTime = reminderTime;
        //Console.WriteLine(newTask.ReminderDateTime.DateTime);
        
        //just get the first list (default Tasks list)
        var lists = await TaskHelper.getClient().Me.Todo.Lists.Request().GetAsync();
        string listID = lists.ElementAt(0).Id;
        //Console.WriteLine("Available Lists: " );

        await CreateTask(newTask, listID);
    }


    private static async Task CreateTask(TodoTask newTask, string listID) {
        await TaskHelper.getClient().Me.Todo
            .Lists[listID]
            .Tasks
            .Request()
            .AddAsync(newTask);
    }

    //Separate Methods for niceness
    private string getDate() {
        string date = Console.ReadLine();
        if (date == "") {
            DateTime newDate;
            newDate = DateTime.Today;

            //Cursed
            if (newDate.Month < 10) {
                return newDate.Year.ToString() + "-0" + newDate.Month.ToString() + "-" + newDate.Day.ToString();
            }

            return newDate.Year.ToString() + "0" + newDate.Month.ToString() + "-" + newDate.Day.ToString();
        }

        try {
            DateTime newDate = Convert.ToDateTime(date);
            if (newDate.Month < 10) {
                //Chaotic formatting stuff
                return newDate.Year.ToString() + "-0" + newDate.Month.ToString() + "-" + newDate.Day.ToString();
            }
            else {
                return newDate.Year.ToString() + "0" + newDate.Month.ToString() + "-" + newDate.Day.ToString();
            }
        }
        catch {
            Console.WriteLine("Try Again");
            return getDate();
        }
    }

    private DateTime getTime() {
        string time = Console.ReadLine();
        DateTime newTime;
        if (time == "") {
            return new DateTime();
        }

        try {
            newTime = Convert.ToDateTime(time);
            return newTime;
        }
        catch {
            Console.WriteLine("Try Again");
            return getTime();
        }
    }

    //No idea what this does
    private IConfigurationRoot LoadAppSettings() {
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