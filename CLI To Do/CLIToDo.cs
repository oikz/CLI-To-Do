using System;
using System.Linq;
using Microsoft.Graph;
using System.Threading.Tasks;

namespace CLI_To_Do;

public static class CLIToDo {
    //Global Variable Goodies
    private const string AppId = "8c6a9efb-30e2-4c95-b975-9a46a82cfaf0"; //Spooky appID
    private static readonly string[] Scopes = { "User.Read", "Tasks.Read", "Tasks.ReadWrite" };


    public static async Task Main(string[] args) {
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
                //await GoogleTasks();
                break;
            case 3:
                Environment.Exit(0);
                break;
        }
    }

    private static async Task MicrosoftToDo() {
        var authProvider = new DeviceCodeAuthProvider(AppId, ToDoScopes);

        // Initialize Graph client
        TaskHelper.Initialize(authProvider);

        //Create new folder for storing data if not already created
        System.IO.Directory.CreateDirectory(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\todo\\");

        // Get signed in user
        var user = TaskHelper.GetMeAsync().Result;
        Console.WriteLine($"Welcome {user.DisplayName}!");

        //make new task because it doesnt work unless i do this or smth
        var newTask = new TodoTask { ODataType = null, Title = UserInterface.GetTitle() };
        
        //ID for the list to add the task to
        var lists = await TaskHelper.GetLists();
        var listID = lists.ElementAt(UserInterface.GetListsHelper(lists.Count) - 1).Id; //Get the chosen list

        //Setup the date and time for the task/reminder

        Console.WriteLine("\nDate: Format: YYYY-MM-DD (Empty for today) ");
        var dateString = UserInterface.GetDate();

        Console.Write("\nTime: (Empty for no reminder) ");
        var newTime = UserInterface.GetTime();

        TaskHelper.SetDates(dateString, newTime, newTask);

        await TaskHelper.CreateTask(newTask, listID);
    }
}