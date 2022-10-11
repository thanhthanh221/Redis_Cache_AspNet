using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Redis_AspNet.Installers
{
    public interface IInstaller
    {
        void InstallService(IServiceCollection services, IConfiguration configuration);
    }
}
