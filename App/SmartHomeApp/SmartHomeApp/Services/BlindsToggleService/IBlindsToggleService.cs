using System.Threading;
using System.Threading.Tasks;

namespace SmartHomeApp.Services.BlindsToggleService
{
    public interface IBlindsToggleService
    {
        Task Toggle();
        Task Toggle(CancellationToken cancellationToken);
    }
}