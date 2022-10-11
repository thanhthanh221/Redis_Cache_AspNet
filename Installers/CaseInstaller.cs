using Redis_AspNet.Configuration;
using Redis_AspNet.Services;
using StackExchange.Redis;

namespace Redis_AspNet.Installers
{
    public class CaseInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            RedisConfiguration redisConfiguration = new RedisConfiguration();
            configuration.GetSection(nameof(RedisConfiguration)).Bind(redisConfiguration);

            services.AddSingleton(redisConfiguration);

            if (!redisConfiguration.Enable)
            {
                return;
            }
            services.AddSingleton<IConnectionMultiplexer>(_
                => ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));

            services.AddStackExchangeRedisCache(option => option.Configuration = redisConfiguration.ConnectionString);
            
            // Quản lí
            services.AddSingleton<IReposeCacheService, ReposeCacheService>();
        }
    }
}
