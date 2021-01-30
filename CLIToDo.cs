using System;
using Microsoft.Graph;
using System.Net.Http.Headers;
using System.Threading.Tasks;

class CLIToDo {
    //Global Variable Goodies
    public static string tenantID = "fce5109a-b406-449c-8c1e-48319af5ab15";
    public static string appID = "0123aef1-0156-455a-a2ce-1e59d4595e79";
    public static string[] graphScopes;
    public static GraphServiceClient graphClient;
    static void Main(string[] args) {
        graphScopes = new[] { "User.Read", "Tasks.ReadWrite" };


        Console.WriteLine("CLI To Do\n");
        TodoTask newTask = new TodoTask();
        Console.WriteLine("Title: ");
        newTask.Title = Console.ReadLine();
        Console.WriteLine("Date: Format: YYYY-MM-DD (Empty for today)");
        //Cursed
        string date;
        DateTime newDate;
        DateTime newTime = new DateTime();
        while (true) {

            date = Console.ReadLine();
            if (date == "") {
                date = DateTime.Today.ToString("yyyy/mm/dd");
                break;
            }
            try {
                newDate = Convert.ToDateTime(date);
                break;
            }
            catch {
                Console.WriteLine("Try Again");
            }

        }
        Console.WriteLine("Time: ");
        while (true) {

            string time = Console.ReadLine();
            /*try {
                newTime.);
                Console.WriteLine(newTime.ToString());
                break;
            }
            catch {
                Console.WriteLine("Try Again");
                Console.WriteLine(newTime.ToString());
            }*/

        }
        DateTimeTimeZone reminderTime = new DateTimeTimeZone();
        reminderTime.TimeZone = "Pacific/Auckland";
        //reminderTime.DateTime =
        //newTask.ReminderDateTime =

    }
}