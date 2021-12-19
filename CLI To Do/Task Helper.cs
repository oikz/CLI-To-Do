using Microsoft.Graph;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CLI_To_Do;

public static class TaskHelper {
    public static GraphServiceClient GraphClient;

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
    /// <param name="listID">The ID of the list it's being added to</param>
    public static async Task CreateTask(TodoTask newTask, string listID) {
        await GraphClient.Me.Todo
            .Lists[listID]
            .Tasks
            .Request()
            .AddAsync(newTask);
        Console.Write("Task Created");
    }

    /// <summary>
    /// Gets the task lists of the current user from Graph API
    /// </summary>
    /// <returns>A page of lists</returns>
    public static async Task<ITodoListsCollectionPage> getLists() {
        var lists = await GraphClient.Me.Todo.Lists.Request().GetAsync();
        Console.WriteLine("Available Lists: ");
        for (var i = 0; i < lists.Count; i++) {
            Console.WriteLine(i + 1 + " " + lists.ElementAt(i).DisplayName);
        }

        return lists;
    }
}