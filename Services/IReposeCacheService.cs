namespace Redis_AspNet.Services
{
    public interface IReposeCacheService
    {
        Task SetCacheReponseAsync(string cacheKey, object response, TimeSpan timeOut);
        Task<String> GetCacheReponseAsync(string cacheKey);
        Task RemoveCacheResponseAsync(string partern);
    }
}
