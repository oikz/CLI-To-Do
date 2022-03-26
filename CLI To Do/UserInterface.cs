using System;

namespace CLI_To_Do;

/// <summary>
/// User interface methods for getting the user's input.
/// </summary>
public static class UserInterface {
    /// <summary>
    /// Prompts the user to choose a platform from those available, looping until they enter a valid input.
    /// </summary>
    /// <returns>The integer index of the chosen platform, with 3 being exit.</returns>
    public static int ChoosePlatform() {
        var choices = new[] {1, 2, 3};
        while (true) {
            var userInput = Console.ReadLine();
            if (int.TryParse(userInput, out var platform) && Array.Exists(choices, e => e == platform)) return platform;
            Console.WriteLine("Please enter a valid number.");
        }
    }

    /// <summary>
    /// Prompts the user to enter a date for the task, displaying a calendar and allowing the user to either enter a
    /// date string or a date shortcut (e.g., a weekday), looping until the input is valid.
    /// </summary>
    /// <returns>The string representation of the user's inputted date.</returns>
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
                Console.ForegroundColor = startOfWeek.Date.Equals(today.Date) ? ConsoleColor.Red : ConsoleColor.Gray;
                Console.Write($"{startOfWeek.Day} ");
                if (startOfWeek.Day < 10) Console.Write(" ");
                startOfWeek = startOfWeek.AddDays(1);
            }
            Console.Write("\n");
        }

        var date = Console.ReadLine();
        
        // Various pre-defined shortcuts for the user to use.
        switch (date.ToLower()) {
            case "":
                return DateTime.Today.ToString("yyyy-MM-dd");
            case "tomorrow":
                var newDate = DateTime.Today;
                newDate = newDate.AddDays(1);
                return newDate.ToString("yyyy-MM-dd");
            case "monday":
                return GetNextWeekday(today.AddDays(1), DayOfWeek.Monday).ToString("yyyy-MM-dd");
            case "tuesday":
                return GetNextWeekday(today.AddDays(1), DayOfWeek.Tuesday).ToString("yyyy-MM-dd");
            case "wednesday":
                return GetNextWeekday(today.AddDays(1), DayOfWeek.Wednesday).ToString("yyyy-MM-dd");
            case "thursday":
                return GetNextWeekday(today.AddDays(1), DayOfWeek.Thursday).ToString("yyyy-MM-dd");
            case "friday":
                return GetNextWeekday(today.AddDays(1), DayOfWeek.Friday).ToString("yyyy-MM-dd");
            case "saturday":
                return GetNextWeekday(today.AddDays(1), DayOfWeek.Saturday).ToString("yyyy-MM-dd");
            case "sunday":
                return GetNextWeekday(today.AddDays(1), DayOfWeek.Sunday).ToString("yyyy-MM-dd");
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
    
    /// <summary>
    /// Finds the date of the next instance of a given day of the week.
    /// Sourced from https://stackoverflow.com/questions/6346119/datetime-get-next-tuesday
    /// </summary>
    /// <param name="start">Date to start from</param>
    /// <param name="day">Day to look for</param>
    /// <returns>The new Date of the specified day</returns>
    private static DateTime GetNextWeekday(DateTime start, DayOfWeek day) {
        var daysToAdd = ((int) day - (int) start.DayOfWeek + 7) % 7;
        return start.AddDays(daysToAdd);
    }

    /// <summary>
    /// Prompts the user to enter a time, returns as a DateTime otherwise loops until the user enters a valid input.
    /// </summary>
    /// <returns>The DateTime representing the user's input.</returns>
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
    
    /// <summary>
    /// Prompts the user to enter a title, returns it as a string otherwise loops until the user enters a valid input.
    /// </summary>
    /// <returns>The Title of the reminder/task to be created.</returns>
    public static string GetTitle() {
        while (true) {
            Console.Write("Title: ");
            var title = Console.ReadLine();
            if (title != "") return title;
            Console.WriteLine("Invalid Title");
        }
    }
    
    /// <summary>
    /// Prompts the user to enter a list id, returns it as an int otherwise loops until the user enters a valid input.
    /// </summary>
    /// <param name="total"></param>
    /// <returns></returns>
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