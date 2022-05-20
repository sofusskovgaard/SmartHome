using System.Windows.Input;
using SmartHomeApp.Infrastructure.TinyIoC;
using SmartHomeApp.Services.AuthenticationService;
using Xamarin.Forms;

namespace SmartHomeApp.ViewModels.Guest
{
    public class LoginViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        
        public LoginViewModel()
        {
            var container = TinyIoCContainer.Current;
            
            _authenticationService = container.Resolve<IAuthenticationService>();
        }

        private Command _login;

        public ICommand Login => _login ??= new Command(() => _authenticationService.Authenticate());
    }
}