using System;
using Microsoft.Graph;
using System.Linq;
using System.Threading.Tasks;

namespace CLI_To_Do;

public static class CLIToDo {
    //Global Variable Goodies
    private static string appID = "8c6a9efb-30e2-4c95-b975-9a46a82cfaf0"; //Spooky appID
    private static readonly string[] scopes = { "User.Read", "Tasks.Read", "Tasks.ReadWrite" };


    public static async Task Main(string[] args) {
        await start();
    }

    //Start the program
    private static async Task start() {
        // Initialize the auth provider with values from appsettings.json
        var authProvider = new DeviceCodeAuthProvider(appID, scopes);

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
        var listID = getLists().Result;

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

    //Method for allowing the user to choose a list from their available lists
    private static async Task<string> getLists() {
        var lists = await TaskHelper.getClient().Me.Todo.Lists.Request().GetAsync();
        Console.WriteLine("Available Lists: ");
        for (var i = 0; i < lists.Count; i++) {
            Console.WriteLine(i + 1 + " " + lists.ElementAt(i).DisplayName);
        }

        var listID = lists.ElementAt(getListsHelper(lists.Count) - 1).Id; //Get the chosen list
        return listID;
    }

    //Helper method for user inputting an int
    private static int getListsHelper(int total) {
        Console.Write("List: ");
        var num = Console.ReadLine();
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