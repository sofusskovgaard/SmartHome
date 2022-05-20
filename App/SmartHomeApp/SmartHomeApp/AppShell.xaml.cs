using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartHomeApp.Infrastructure.TinyIoC;
using SmartHomeApp.Services.AuthenticationService;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartHomeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private Command _logoutCommand;

        public ICommand LogoutCommand => _logoutCommand ??=
            new Command(() => TinyIoCContainer.Current.Resolve<IAuthenticationService>().Destroy());
    }
}