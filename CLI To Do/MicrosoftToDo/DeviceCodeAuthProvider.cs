using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using File = System.IO.File;

namespace CLI_To_Do.MicrosoftToDo;

public class DeviceCodeAuthProvider : IAuthenticationProvider {
    private readonly IPublicClientApplication _msalClient;
    private readonly string[] _scopes;
    private IAccount _userAccount;

    public DeviceCodeAuthProvider(string appId, string[] scopes) {
        _scopes = scopes;

        _msalClient = PublicClientApplicationBuilder
            .Create(appId)
            .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount)
            .WithRedirectUri("http://localhost:8383")
            .Build();
        TokenCacheHelper.EnableSerialization(_msalClient.UserTokenCache);
    }

    /// <summary>
    /// Retrieves a token from the local cache if available or begins the authentication flow, opening a web browser
    /// and prompting the user to login, then saving the token locally for later use.
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetAccessToken() {
        //First tries to get a token from the cache
        try {
            var previousLogin = await File.ReadAllTextAsync(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                "\\todo\\prevUser.txt");
            previousLogin = previousLogin.Split("\r")[0]; //Evil formatting
            previousLogin = previousLogin.Split("\n")[0]; //Evil formatting again....
            var result = await _msalClient
                .AcquireTokenSilent(_scopes, previousLogin)
                .ExecuteAsync();
            return result.AccessToken;
        } catch (Exception) {
            // If there is no saved user account, the user must sign-in
            AuthenticationResult result;
            if (_userAccount == null) {
                try {
                    // Let user sign in
                    result = await _msalClient.AcquireTokenInteractive(_scopes).ExecuteAsync();
                    _userAccount = result.Account;
                    string[] lines = { _userAccount.Username };
                    File.WriteAllLines(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                        "\\todo\\prevUser.txt",
                        lines); //Questionable saving of previous user but its just a username and is local so should be fine
                    return result.AccessToken;
                } catch (Exception exception) {
                    Console.WriteLine($"Error getting access token: {exception.Message}");
                    return null;
                }
            }

            // If there is an account, call AcquireTokenSilent
            // By doing this, MSAL will refresh the token automatically if
            // it is expired. Otherwise it returns the cached token.
            result = await _msalClient
                .AcquireTokenSilent(_scopes, _userAccount)
                .ExecuteAsync();

            return result.AccessToken;
        }
    }

    // This is the required function to implement IAuthenticationProvider
    // The Graph SDK will call this function each time it makes a Graph
    // call.
    public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage) {
        requestMessage.Headers.Authorization =
            new AuthenticationHeaderValue("bearer", await GetAccessToken());
    }
}