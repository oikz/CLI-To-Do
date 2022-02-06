using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Graph;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Util.Store;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Task = System.Threading.Tasks.Task;

namespace CLI_To_Do;

public static class CLIToDo {
    private const string ToDoAppId = "8c6a9efb-30e2-4c95-b975-9a46a82cfaf0"; //Spooky appID
    private const string GoogleJson =
        "{\"installed\":{\"client_id\":\"82771178022-gba3dto0pftqk8n9nd6lell0ko8jrler.apps.googleusercontent.com\",\"project_id\":\"cli-to-do\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"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\",\"redirect_uris\":[\"urn:ietf:wg:oauth:2.0:oob\",\"http://localhost\"]}}";
    private static readonly string[] ToDoScopes = { "User.Read", "Tasks.Read", "Tasks.ReadWrite" };
    private static readonly string[] GoogleScopes = { TasksService.Scope.Tasks };
    
    public static async Task Main(string[] args) {
        await ChoosePlatform();
    }

    private static async Task ChoosePlatform() {
        Console.WriteLine("Welcome to CLI To Do");
        Console.WriteLine("Please choose a platform to use: ");
        Console.WriteLine("1. Microsoft To Do");
        Console.WriteLine("2. Google Tasks");
        Console.WriteLine("3. Exit");
        var input = UserInterface.ChoosePlatform();
        switch (input) {
            case 1:
                await MicrosoftToDo();
                break;
            case 2:
                await GoogleTasks();
                break;
            case 3:
                Environment.Exit(0);
                break;
        }
    }

    private static async Task GoogleTasks() {
        UserCredential credential;
        await using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(GoogleJson))) {
            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            var credPath = "token.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStreamAsync(stream).Result.Secrets,
                GoogleScopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credPath, true)).Result;
            Console.WriteLine("Credential file saved to: " + credPath);
        }

        // Create Google Tasks API service.
        var service = new TasksService(new BaseClientService.Initializer {
            HttpClientInitializer = credential,
            ApplicationName = "CLI To Do"
        });

        // Define parameters of request.
        var listRequest = service.Tasklists.List();
        listRequest.MaxResults = 10;

        // List task lists.
        var taskLists = listRequest.Execute().Items;
        Console.WriteLine("Task Lists:");
        if (taskLists != null && taskLists.Count > 0) {
            foreach (var taskList in taskLists) {
                Console.WriteLine("{0} ({1})", taskList.Title, taskList.Id);
            }
        } else {
            Console.WriteLine("No task lists found.");
        }

        Console.Read();
    }

    private static async Task MicrosoftToDo() {
        var authProvider = new DeviceCodeAuthProvider(ToDoAppId, ToDoScopes);

        // Initialize Graph client
        TaskHelper.Initialize(authProvider);

        //Create new folder for storing data if not already created
        System.IO.Directory.CreateDirectory(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\todo\\");

        // Get signed in user
        var user = TaskHelper.GetMeAsync().Result;
        Console.WriteLine($"Welcome {user.DisplayName}!");

        //make new task because it doesnt work unless i do this or smth
        var newTask = new TodoTask { ODataType = null, Title = UserInterface.GetTitle() };

        //ID for the list to add the task to
        var lists = await TaskHelper.GetLists();
        var listID = lists.ElementAt(UserInterface.GetListsHelper(lists.Count) - 1).Id; //Get the chosen list

        //Setup the date and time for the task/reminder

        Console.WriteLine("\nDate: Format: YYYY-MM-DD (Empty for today) ");
        var dateString = UserInterface.GetDate();

        Console.Write("\nTime: (Empty for no reminder) ");
        var newTime = UserInterface.GetTime();

        TaskHelper.SetDates(dateString, newTime, newTask);

        await TaskHelper.CreateTask(newTask, listID);
    }
}