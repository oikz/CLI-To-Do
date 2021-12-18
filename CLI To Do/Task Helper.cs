using Microsoft.Graph;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CLI_To_Do;

public static class TaskHelper {
    private static GraphServiceClient _graphClient;

    public static void Initialize(IAuthenticationProvider authProvider) {
        _graphClient = new GraphServiceClient(authProvider);
    }

    public static async Task<User> GetMeAsync() {
        try {
            // GET /me
            return await _graphClient.Me
                .Request()
                .Select(u => new {
                    u.DisplayName,
                })
                .GetAsync();
        } catch (ServiceException ex) {
            Console.WriteLine($"Error getting signed-in user: {ex.Message}");
            return null;
        }
    }

    public static async Task CreateTask(TodoTask newTask, string listID) {
        await _graphClient.Me.Todo
            .Lists[listID]
            .Tasks
            .Request()
            .AddAsync(newTask);
        Console.Write("Task Created");
    }

    //Method for allowing the user to choose a list from their available lists
    public static async Task<string> getLists() {
        var lists = await _graphClient.Me.Todo.Lists.Request().GetAsync();
        Console.WriteLine("Available Lists: ");
        for (var i = 0; i < lists.Count; i++) {
            Console.WriteLine(i + 1 + " " + lists.ElementAt(i).DisplayName);
        }

        var listID = lists.ElementAt(UserInterface.GetListsHelper(lists.Count) - 1).Id; //Get the chosen list
        return listID;
    }
}