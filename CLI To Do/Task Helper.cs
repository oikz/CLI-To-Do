using Microsoft.Graph;
using System;
using System.Threading.Tasks;

namespace CLI_To_Do;

//Entirely Snatched from MS tutorial
public class TaskHelper {
    private static GraphServiceClient graphClient;
    public static void Initialize(IAuthenticationProvider authProvider) {
        graphClient = new GraphServiceClient(authProvider);
    }

    public static async Task<User> GetMeAsync() {
        try {
            // GET /me
            return await graphClient.Me
                .Request()
                .Select(u => new {
                    u.DisplayName,
                })
                .GetAsync();
        }
        catch (ServiceException ex) {
            Console.WriteLine($"Error getting signed-in user: {ex.Message}");
            return null;
        }
    }
    
    public static GraphServiceClient getClient() {
        return graphClient;
    }
}