using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartHomeApp.Dtos.DataApiService;
using SmartHomeApp.Infrastructure.HttpService;
using SmartHomeApp.Infrastructure.TinyIoC;
using SmartHomeApp.Services.AuthenticationService;

namespace SmartHomeApp.Services.BlindsToggleService
{
    public class BlindsToggleService : IBlindsToggleService
    {
        private readonly IHttpService _httpService;
        private readonly IAuthenticationService _authenticationService;

        public BlindsToggleService()
        {
            var container = TinyIoCContainer.Current;

            _httpService = container.Resolve<IHttpService>();
            _authenticationService = container.Resolve<IAuthenticationService>();
        }

        public Task Toggle()
        {
            return Toggle(CancellationToken.None);
        }
        
        public async Task Toggle(CancellationToken cancellationToken)
        {
            await _httpService.PostAsync("http://localhost:4001/api/blinds/toggle", _authenticationService.Tokens[AuthenticationService.AuthenticationService.ACCESS_TOKEN], cancellationToken: cancellationToken);
        }
    }
}