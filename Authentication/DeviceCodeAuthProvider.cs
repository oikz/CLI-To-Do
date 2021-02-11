using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using File = System.IO.File;

public class DeviceCodeAuthProvider : IAuthenticationProvider {
    private IPublicClientApplication _msalClient;
    private string[] _scopes;
    private IAccount _userAccount;

    public DeviceCodeAuthProvider(string appId, string[] scopes) {
        _scopes = scopes;

        _msalClient = PublicClientApplicationBuilder
            .Create(appId)
            .WithAuthority(AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount, true)
            .WithRedirectUri("http://localhost:8383")
            .Build();
        TokenCacheHelper.EnableSerialization(_msalClient.UserTokenCache);
    }

    public async Task<string> GetAccessToken() {
        //First tries to get a token from the cache
        try {
            string previousLogin = "";
            previousLogin = await File.ReadAllTextAsync(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) +
                "\\todo\\prevUser.txt");
            previousLogin = previousLogin.Split("\r")[0]; //Evil formatting
            previousLogin = previousLogin.Split("\n")[0]; //Evil formatting again....
            var result = await _msalClient
                .AcquireTokenSilent(_scopes, previousLogin)
                .ExecuteAsync();
            return result.AccessToken;
        } catch (Exception) {
            // If there is no saved user account, the user must sign-in
            if (_userAccount == null) {
                try {
                    // Let user sign in
                    var result = await _msalClient.AcquireTokenInteractive(_scopes).ExecuteAsync();
                    _userAccount = result.Account;
                    string[] lines = {_userAccount.Username};
                    File.WriteAllLines(
                        System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) +
                        "\\todo\\prevUser.txt",
                        lines); //Questionable saving of previous user but its just a username and is local so should be fine
                    return result.AccessToken;
                } catch (Exception exception) {
                    Console.WriteLine($"Error getting access token: {exception.Message}");
                    return null;
                }
            } else {
                // If there is an account, call AcquireTokenSilent
                // By doing this, MSAL will refresh the token automatically if
                // it is expired. Otherwise it returns the cached token.
                var result = await _msalClient
                    .AcquireTokenSilent(_scopes, _userAccount)
                    .ExecuteAsync();

                return result.AccessToken;
            }
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