using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SmartHomeApp.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly OidcClient _client;
        
        private object _refresh_locker;

        public const string ACCESS_TOKEN = "accessToken";
        public const string REFRESH_TOKEN = "refreshToken";
        public const string IDENTITY_TOKEN = "identityToken";
        
        private DateTimeOffset? _expiresAt;

        /// <summary>
        /// Seconds until access token expires
        /// </summary>
        public int? ExpiresInSeconds => _expiresAt?.CompareTo(DateTimeOffset.Now);

        private bool? _loggedIn;
        public bool LoggedIn
        {
            get
            {
                if (_loggedIn == null)
                {
                    if (Tokens.ContainsKey(REFRESH_TOKEN)) return _loggedIn ??= true;
                    
                    var refreshToken = SecureStorage.GetAsync(REFRESH_TOKEN).GetAwaiter().GetResult();

                    if (!string.IsNullOrEmpty(refreshToken))
                    {
                        var accessToken = SecureStorage.GetAsync(ACCESS_TOKEN).GetAwaiter().GetResult();
                        var identityToken = SecureStorage.GetAsync(IDENTITY_TOKEN).GetAwaiter().GetResult();

                        Tokens[REFRESH_TOKEN] = refreshToken;
                        Tokens[ACCESS_TOKEN] = accessToken;
                        Tokens[IDENTITY_TOKEN] = identityToken;

                        return _loggedIn ??= true;
                    }
                }

                return _loggedIn ??= false;
            }
            private set
            {
                MessagingCenter.Send(this, value ? "logged_in" : "logged_out");
                _loggedIn = value;
            }
        }

        private Dictionary<string, string> _tokens = new Dictionary<string, string>();

        public Dictionary<string, string> Tokens => _tokens;

        public AuthenticationService()
        {
            var browser = DependencyService.Get<IBrowser>();
            var options = new OidcClientOptions
            {
                Authority = $"https://{AuthConfig.Domain}",
                ClientId = AuthConfig.ClientId,
                Scope = AuthConfig.Scopes,
                RedirectUri = $"{AuthConfig.PackageName}://{AuthConfig.Domain}/ios/{AuthConfig.PackageName}/callback",
                PostLogoutRedirectUri = $"{AuthConfig.PackageName}://{AuthConfig.Domain}/ios/{AuthConfig.PackageName}/callback",
                Browser = browser
            };

            _client = new OidcClient(options);
        }

        public async Task<bool> Authenticate()
        {
            try
            {
                var audience = new Dictionary<string, string>
                {
                    { "audience", AuthConfig.Audience }
                };

                var loginRequest = new LoginRequest() { FrontChannelExtraParameters = new Parameters(audience) };
                
                var result = await _client.LoginAsync(loginRequest);
                LoggedIn = await _setTokens(result);
                return LoggedIn;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task RefreshToken()
        {
            if (LoggedIn && ExpiresInSeconds is < 120)
            {
                var result = await _client.RefreshTokenAsync(_tokens[REFRESH_TOKEN]);
                await _setTokens(result);
            }
        }

        public async Task<bool> Destroy()
        {
            try
            {
                var url = $"https://{AuthConfig.Domain}/v2/logout?client_id={AuthConfig.ClientId}&returnTo={UrlEncoder.Default.Encode(_client.Options.PostLogoutRedirectUri)}";

                var browserOptions = new BrowserOptions(url, _client.Options.PostLogoutRedirectUri ?? string.Empty);
                var res = await _client.Options.Browser.InvokeAsync(browserOptions);

                if (!res.IsError)
                {
                    _removeTokens();
                    
                    LoggedIn = false;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private async Task<bool> _setTokens(LoginResult result)
        {
            if (result.IsError) return false;

            _expiresAt = result.AccessTokenExpiration;
            
            await SecureStorage.SetAsync(ACCESS_TOKEN, result.AccessToken);
            _tokens[ACCESS_TOKEN] = result.AccessToken;
                    
            await SecureStorage.SetAsync(REFRESH_TOKEN, result.RefreshToken);
            _tokens[REFRESH_TOKEN] = result.RefreshToken;
                    
            await SecureStorage.SetAsync(IDENTITY_TOKEN, result.IdentityToken);
            _tokens[IDENTITY_TOKEN] = result.IdentityToken;
            
            return true;
        }
        
        private async Task<bool> _setTokens(RefreshTokenResult result)
        {
            if (result.IsError) return false;
            
            _expiresAt = result.AccessTokenExpiration;
            
            await SecureStorage.SetAsync(ACCESS_TOKEN, result.AccessToken);
            _tokens[ACCESS_TOKEN] = result.AccessToken;
                    
            await SecureStorage.SetAsync(REFRESH_TOKEN, result.RefreshToken);
            _tokens[REFRESH_TOKEN] = result.RefreshToken;
                    
            await SecureStorage.SetAsync(IDENTITY_TOKEN, result.IdentityToken);
            _tokens[IDENTITY_TOKEN] = result.IdentityToken;

            return true;
        }

        private void _removeTokens()
        {
            _expiresAt = null;
            SecureStorage.Remove(REFRESH_TOKEN);
            SecureStorage.Remove(ACCESS_TOKEN);
            SecureStorage.Remove(IDENTITY_TOKEN);
            _tokens.Clear();
        }
    }
}