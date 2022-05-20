using System;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using SmartHomeApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SmartHomeApp.Infrastructure;
using SmartHomeApp.Infrastructure.HttpService;
using SmartHomeApp.Infrastructure.TinyIoC;
using SmartHomeApp.Services.AuthenticationService;
using SmartHomeApp.Services.BlindsToggleService;
using SmartHomeApp.Services.DataApiService;
using Xamarin.Forms.Internals;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SmartHomeApp
{
    public partial class App : Application
    {
        public App()
        {
            _RegisterContainer();
            InitializeComponent();

            if (TinyIoCContainer.Current.Resolve<IAuthenticationService>().LoggedIn)
            {
                MainPage = new AppShell();   
            }
            else
            {
                MainPage = new GuestShell();
            }

            MessagingCenter.Subscribe<AuthenticationService>(this, "logged_in", service => MainPage = new AppShell());
            MessagingCenter.Subscribe<AuthenticationService>(this, "logged_out", service => MainPage = new GuestShell());
        }

        private void _RegisterContainer()
        {
            var container = TinyIoCContainer.Current;

            container.Register((_, __) => new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), "cache").AsSingleton();
            
            container.Register<IHttpService, HttpService>().AsSingleton();
            container.Register<IDataApiService, DataApiService>().AsSingleton();
            container.Register<IBlindsToggleService, BlindsToggleService>().AsSingleton();

            container.Register<IAuthenticationService, AuthenticationService>().AsSingleton();
        }

        protected override void OnResume()
        {
            TinyIoCContainer.Current.Resolve<IAuthenticationService>().RefreshToken();
        }

        protected override void OnStart()
        {
            TinyIoCContainer.Current.Resolve<IAuthenticationService>().RefreshToken();
        }
    }
}
