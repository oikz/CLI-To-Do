using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace CLI_To_Do.MicrosoftToDo;

public static class ToDoTaskHelper {
    public static GraphServiceClient GraphClient { get; set; }

    public static void Initialize(IAuthenticationProvider authProvider) {
        GraphClient = new GraphServiceClient(authProvider);
    }

    /// <summary>
    /// Gets the currently logged in User from Graph
    /// </summary>
    /// <returns>The User Object</returns>
    public static async Task<User> GetMeAsync() {
        try {
            // GET /me
            return await GraphClient.Me
                .Request()
                .GetAsync();
        } catch (ServiceException ex) {
            Console.WriteLine($"Error getting signed-in user: {ex.Message}");
            return null;
        }
    }

    
    /// <summary>
    /// Creates a task and POSTs it to Graph API
    /// </summary>
    /// <param name="newTask">The task being created</param>
    /// <param name="listId">The ID of the list it's being added to</param>
    public static async Task CreateTask(TodoTask newTask, string listId) {
        await GraphClient.Me.Todo
            .Lists[listId]
            .Tasks
            .Request()
            .AddAsync(newTask);
        Console.Write("Task Created");
    }

    /// <summary>
    /// Gets the task lists of the current user from Graph API
    /// </summary>
    /// <returns>A page of lists</returns>
    public static async Task<ITodoListsCollectionPage> GetLists() {
        var lists = await GraphClient.Me.Todo.Lists.Request().GetAsync();
        Console.WriteLine("\nAvailable Lists: ");
        for (var i = 0; i < lists.Count; i++) {
            Console.WriteLine(i + 1 + " " + lists.ElementAt(i).DisplayName);
        }

        return lists;
    }
    
    /// <summary>
    /// Set the task due date and reminder date for the TodoTask
    /// </summary>
    /// <param name="dateString">The date it is due, represented in a string</param>
    /// <param name="newTime">The time to be reminded</param>
    /// <param name="newTask">The TodoTask to be updated</param>
    public static void SetDates(string dateString, DateTime newTime, TodoTask newTask) {
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