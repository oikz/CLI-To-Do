using System;
using Microsoft.Graph;
using System.Linq;
using System.Threading.Tasks;

namespace CLI_To_Do;

class CLIToDo {
    //Global Variable Goodies
    private static string appID = "8c6a9efb-30e2-4c95-b975-9a46a82cfaf0"; //Spooky appID
    private static readonly string[] scopes = {"User.Read", "Tasks.Read", "Tasks.ReadWrite"};


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
        var newTask = new TodoTask {ODataType = null, Title = getTitle()};

        //Get title

        //ID for the list to add the task to
        var listID = getLists().Result;

        //Setup the date and time for the task/reminder

        Console.WriteLine("Date: Format: YYYY-MM-DD (Empty for today)");
        var dateString = getDate();

        Console.Write("Time: ");
        var newTime = getTime();

        //Set all the juicy task info
        var reminderTime = new DateTimeTimeZone {
            ODataType = null,//Required for whatever reason
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

    //Separate Methods for niceness
    private static string getDate() {
        var date = Console.ReadLine();
        
        //Empty for today
        if (date == "") {
            var newDate = DateTime.Today;

            //Cursed formatting of months
            if (newDate.Month < 10) {
                return newDate.Year + "-0" + newDate.Month + "-" + newDate.Day;
            }

            return newDate.Year + "-" + newDate.Month + "-" + newDate.Day;
        }

        //Quick shortcut for tomorrow
        if (date.ToLower() == "tomorrow") {
            var newDate = DateTime.Today;
            newDate = newDate.AddDays(1);

            //Cursed again
            if (newDate.Month < 10) {
                return newDate.Year + "-0" + newDate.Month + "-" + newDate.Day;
            }

            return newDate.Year + "-" + newDate.Month + "-" + newDate.Day;
        }

        //Attempt to create a DateTime based on the user's inputs
        try {
            var newDate = Convert.ToDateTime(date);
            if (newDate.Month < 10) {
                //Chaotic formatting stuff
                return newDate.Year + "-0" + newDate.Month + "-" + newDate.Day;
            }

            return newDate.Year + "-" + newDate.Month + "-" + newDate.Day;
        } catch {
            Console.WriteLine("Try Again");
            return getDate();
        }
    }

    private static DateTime getTime() {
        var time = Console.ReadLine();
        if (time == "") {
            return new DateTime();
        }

        try {
            var newTime = Convert.ToDateTime(time);
            return newTime;
        } catch {
            Console.WriteLine("Try Again");
            return getTime();
        }
    }

    //Separate method to account for empty titles more easily
    //Loop until valid title entered
    private static string getTitle() {
        while (true) {
            Console.Write("Title: ");
            var title = Console.ReadLine();
            if (title != "") return title;
            Console.WriteLine("Invalid Title");
        }
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