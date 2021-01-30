using System;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;

class CLIToDo {
    //Global Variable Goodies
    public static string tenantID = "fce5109a-b406-449c-8c1e-48319af5ab15";//Spooky tenantID
    public static string appID = "0123aef1-0156-455a-a2ce-1e59d4595e79";
    public static string[] graphScopes;
    public static GraphServiceClient graphClient;

    static void Main(string[] args) {
        CLIToDo instance = new CLIToDo();
        instance.start();
    }

    //Start the program
    private void start() {
        graphScopes = new[] { "User.Read", "Tasks.ReadWrite" };


        Console.WriteLine("CLI To Do\n");
        TodoTask newTask = new TodoTask();
        Console.WriteLine("Title: ");
        newTask.Title = Console.ReadLine();
        Console.WriteLine("Date: Format: YYYY-MM-DD (Empty for today)");
        //Cursed
        string date;

        //VS was complaining about uninitialized vars
        DateTime newDate = new DateTime();
        DateTime newTime = new DateTime();

        newDate = getDate();
        Console.WriteLine("Time: ");
        newTime = getTime();

        DateTimeTimeZone reminderTime = new DateTimeTimeZone();
        reminderTime.TimeZone = "Pacific/Auckland";
        reminderTime.DateTime = newDate.ToString() + "T" + newTime.ToString();
        newTask.ReminderDateTime = reminderTime;

    }

    //Separate Methods for niceness
    static private DateTime getDate() {
        string date = Console.ReadLine();
        if (date == "") {
            return DateTime.Today;
        }
        try {
            DateTime newDate = Convert.ToDateTime(date);
            return newDate;
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
}