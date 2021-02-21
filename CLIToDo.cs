using System;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class CLIToDo {
    //Global Variable Goodies
    public static string tenantID = "fce5109a-b406-449c-8c1e-48319af5ab15"; //Spooky tenantID
    public static string appID = "8c6a9efb-30e2-4c95-b975-9a46a82cfaf0"; //Spooky appID
    public static string[] scopes = {"User.Read", "Tasks.Read", "Tasks.ReadWrite"};


    static async Task Main(string[] args) {
        CLIToDo instance = new CLIToDo();
        await instance.start();
    }

    //Start the program
    private async Task start() {
        // Initialize the auth provider with values from appsettings.json
        var authProvider = new DeviceCodeAuthProvider(appID, scopes);

        // Initialize Graph client
        TaskHelper.Initialize(authProvider);

        //Create new folder for storing data if not already created
        System.IO.Directory.CreateDirectory(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\todo\\");

        // Get signed in user
        var user = TaskHelper.GetMeAsync().Result;
        Console.WriteLine($"Welcome {user.DisplayName}!");

        //make new task because it doesnt work unless i do this or smth
        var newTask = new TodoTask {
            ODataType = null
        };

        //Get title
        newTask.Title = getTitle();

        //ID for the list to add the task to
        string listID = getLists().Result;

        //Setup the date and time for the task/reminder
        DateTime newTime;

        Console.WriteLine("Date: Format: YYYY-MM-DD (Empty for today)");
        string dateString = getDate();

        Console.Write("Time: ");
        newTime = getTime();

        //Set all the juicy task info
        var reminderTime = new DateTimeTimeZone();
        reminderTime.ODataType = null; //Required for whatever reason
        //reminderTime.TimeZone = "Pacific/Auckland";
        reminderTime.TimeZone = TimeZoneInfo.Local.StandardName;
        reminderTime.DateTime = dateString + "T" + newTime.TimeOfDay.ToString() + ".0000000";
        newTask.ReminderDateTime = reminderTime;

        //No set time
        if (newTime.TimeOfDay.ToString() != "") {
            newTask.ReminderDateTime = reminderTime;
        }

        newTask.DueDateTime = reminderTime; //Set the due date for the reminder

        await CreateTask(newTask, listID);
    }


    private static async Task CreateTask(TodoTask newTask, string listID) {
        await TaskHelper.getClient().Me.Todo
            .Lists[listID]
            .Tasks
            .Request()
            .AddAsync(newTask);
        Console.Write("Task Created");
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

        //quick shortcut for tomorrow
        if (date.ToLower() == "tomorrow") {
            DateTime newDate = DateTime.Today;
            newDate = newDate.AddDays(1);

            //Cursed again
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

            return newDate.Year.ToString() + "0" + newDate.Month.ToString() + "-" + newDate.Day.ToString();
        } catch {
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
        } catch {
            Console.WriteLine("Try Again");
            return getTime();
        }
    }

    //Separate method to account for empty titles easierly
    private string getTitle() {
        Console.Write("Title: ");
        string title = Console.ReadLine();
        if (title != "") return title;
        Console.WriteLine("Invalid Title");
        return getTitle();
    }

    //Method for allowing the user to choose a list from their available lists
    private async Task<string> getLists() {
        var lists = await TaskHelper.getClient().Me.Todo.Lists.Request().GetAsync();
        Console.WriteLine("Available Lists: ");
        for (int i = 0; i < lists.Count; i++) {
            Console.WriteLine(i + 1 + " " + lists.ElementAt(i).DisplayName);
        }

        string listID = lists.ElementAt(getListsHelper(lists.Count) - 1).Id; //Get the chosen list
        return listID;
    }

    //Helper method for user inputting an int
    private int getListsHelper(int total) {
        Console.Write("List: ");
        string num = Console.ReadLine();
        if (num == "") {
            return 1; //Return default list if the user presses enter
        }

        int index;
        try {
            index = Convert.ToInt32(num);
            if (index <= 0 || index > total) {
                //Out of range checks
                Console.WriteLine("Invalid choice");
                return getListsHelper(total);
            }
        } catch (Exception) {
            //Try catch for not integers
            Console.WriteLine("Not an integer");
            return getListsHelper(total);
        }

        return index;
    }
}