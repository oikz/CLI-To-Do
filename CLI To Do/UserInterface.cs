using System;

namespace CLI_To_Do;

/// <summary>
/// User interface methods for getting the user's input.
/// </summary>
public static class UserInterface {
    public static string getDate() {
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

    public static DateTime getTime() {
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
    public static string getTitle() {
        while (true) {
            Console.Write("Title: ");
            var title = Console.ReadLine();
            if (title != "") return title;
            Console.WriteLine("Invalid Title");
        }
    }
}