using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Util.Store;

namespace CLI_To_Do.GoogleTasks; 

public static class GoogleTaskHelper {

    /// <summary>
    /// Create a new Google TasksService to be used to create tasks 
    /// </summary>
    /// <param name="json">The JSON required to authorize and create the service</param>
    /// <param name="scopes">The scopes that this service is allowed to use</param>
    /// <returns>The created Service</returns>
    public static async Task<TasksService> CreateService(string json, IEnumerable<string> scopes) {
        UserCredential credential;
        await using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json))) {
            var credPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\todo\\token.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStreamAsync(stream).Result.Secrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
        }

        // Create Google Tasks API service.
        var service = new TasksService(new BaseClientService.Initializer {
            HttpClientInitializer = credential,
            ApplicationName = "CLI To Do"
        });
        return service;
    }

    /// <summary>
    /// Gets the tasks lists of the current user from the Google Tasks API
    /// </summary>
    /// <param name="service">The service required to send the API Requets</param>
    /// <returns>A list of TaskLists</returns>
    public static IList<TaskList> GetLists(TasksService service) {
        // Define parameters of request.
        var listRequest = service.Tasklists.List();
        listRequest.MaxResults = 10;

        // List task lists.
        var taskLists = listRequest.Execute().Items;
        Console.WriteLine("\nAvailable Lists:");
        for (var i = 0; i < taskLists.Count; i++) {
            Console.WriteLine(i + 1 + " " + taskLists.ElementAt(i).Title);
        }
        
        return taskLists.ToList();
    }
}