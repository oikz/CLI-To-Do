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
        //Console.WriteLine(newTask.DueDateTime.ToString());

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
}