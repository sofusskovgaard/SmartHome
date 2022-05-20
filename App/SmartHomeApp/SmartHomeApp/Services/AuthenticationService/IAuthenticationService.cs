using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHomeApp.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        bool LoggedIn { get; }
        
        Dictionary<string, string> Tokens { get; }
        
        Task<bool> Authenticate();
        
        Task RefreshToken();

        Task<bool> Destroy();
    }
}