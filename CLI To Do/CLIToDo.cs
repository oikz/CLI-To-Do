using System;
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

        Console.Write("Time: (Empty for no reminder)");
        var newTime = UserInterface.getTime();

        SetDates(dateString, newTime, newTask);

        await TaskHelper.CreateTask(newTask, listID);
    }

    /// <summary>
    /// Set the task due date and reminder date for the TodoTask
    /// </summary>
    /// <param name="dateString">The date it is due, represented in a string</param>
    /// <param name="newTime">The time to be reminded</param>
    /// <param name="newTask">The TodoTask to be updated</param>
    private static void SetDates(string dateString, DateTime newTime, TodoTask newTask) {
        //Set all the juicy task info
        var reminderTime = new DateTimeTimeZone {
            ODataType = null, //Required for whatever reason
            TimeZone = TimeZoneInfo.Local.StandardName,
            DateTime = dateString + "T" + newTime.TimeOfDay + ".0000000"
        };

        //Only create reminder if date is not empty
        if (newTime.TimeOfDay != new DateTime().TimeOfDay) {
            newTask.ReminderDateTime = reminderTime;
        }

        newTask.DueDateTime = reminderTime; //Set the due date for the reminder
    }
}