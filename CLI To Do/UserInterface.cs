using System;

namespace CLI_To_Do;

/// <summary>
/// User interface methods for getting the user's input.
/// </summary>
public static class UserInterface {
    public static string GetDate() {
        //Display Calendar
        var today = DateTime.Today;
        var startOfWeek = DateTime.Parse($"{today.Year}-{today.Month}-{01}");
        
        if (!startOfWeek.Date.DayOfWeek.Equals(DayOfWeek.Monday)) {
            startOfWeek = startOfWeek.AddDays(-(int)startOfWeek.Date.DayOfWeek + 1);
        }

        Console.WriteLine(today.ToString("MMMM"));
        Console.WriteLine("M  T  W  T  F  S  S");
        Console.WriteLine("___________________");
        for (var j = 0; j < 5; j++) {
            for (var i = 0; i < 7; i++) {
                Console.ForegroundColor = startOfWeek.Date.Equals(today.Date) ? ConsoleColor.Red : ConsoleColor.White;
                Console.Write($"{startOfWeek.Day} ");
                if (startOfWeek.Day < 10) Console.Write(" ");
                startOfWeek = startOfWeek.AddDays(1);
            }
            Console.Write("\n");
        }

        var date = Console.ReadLine();

        //Empty for today
        if (date == "") {
            return DateTime.Today.ToString("yyyy-MM-dd");
        }

        //Quick shortcut for tomorrow
        if (date.ToLower() == "tomorrow") {
            var newDate = DateTime.Today;
            newDate = newDate.AddDays(1);
            return newDate.ToString("yyyy-MM-dd");
        }

        //Attempt to create a DateTime based on the user's inputs
        try {
            var newDate = Convert.ToDateTime(date);
            return newDate.ToString("yyyy-MM-dd");
        } catch {
            Console.WriteLine("Try Again");
            return GetDate();
        }
    }

    public static DateTime GetTime() {
        var time = Console.ReadLine();
        if (time == "") {
            return new DateTime(); //Empty for no reminder
        }

        try {
            var newTime = Convert.ToDateTime(time);
            return newTime;
        } catch {
            Console.WriteLine("Try Again");
            return GetTime();
        }
    }

    //Separate method to account for empty titles more easily
    //Loop until valid title entered
    public static string GetTitle() {
        while (true) {
            Console.Write("Title: ");
            var title = Console.ReadLine();
            if (title != "") return title;
            Console.WriteLine("Invalid Title");
        }
    }
    
    //Helper method for user inputting an int
    public static int GetListsHelper(int total) {
        Console.Write("List: ");
        var num = Console.ReadLine();
        if (string.IsNullOrEmpty(num)) {
            return 1; //Return default list if the user presses enter
        }

        int index;
        try {
            index = Convert.ToInt32(num);
            if (index <= 0 || index > total) {
                //Out of range checks
                Console.WriteLine("Invalid choice");
                return GetListsHelper(total);
            }
        } catch (Exception) {
            //Try catch for not integers
            Console.WriteLine("Not an integer");
            return GetListsHelper(total);
        }

        return index;
    }
}