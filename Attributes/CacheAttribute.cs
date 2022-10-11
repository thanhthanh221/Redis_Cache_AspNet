using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Redis_AspNet.Configuration;
using Redis_AspNet.Services;

namespace Redis_AspNet.Attributes
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int timeToLiveSeconds;

        public CacheAttribute(int timeToLiveSeconds = 1000)
        {
            this.timeToLiveSeconds = timeToLiveSeconds;
        }
        //Action trước khi check
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Xử lý Catche
            RedisConfiguration cacheConfiguration =
                context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();

            //Xem Catche có hay chưa => nếu không có Catche thì bỏ qua  => tới Action Controller
            if (!cacheConfiguration.Enable)
            {
                await next(); // chạy vào controller xử lý
                return;
            }
            var cacheService =
                context.HttpContext.RequestServices.GetRequiredService<IReposeCacheService>();

            // Tạo ra key dựa vào tham số truyền vào
            String cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            // Lấy Catche
            String cacheRepose = await cacheService.GetCacheReponseAsync(cacheKey); 

            // Nếu có Catche thì trả về Catche luôn -- Mã trả về 200 -- Không trả về Action Method
            if (!string.IsNullOrEmpty(cacheRepose))
            {
                ContentResult contentResult = new ContentResult
                {
                    Content = cacheRepose,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            // Nếu Catche rỗng thì gọi đến Action Method của Controller

            ActionExecutedContext excutedContext = await next(); // Repone trả về

            if (excutedContext.Result is OkObjectResult objectResult) // nếu trả về hợp lệ mã 200
            {
                await cacheService
                    .SetCacheReponseAsync(cacheKey, objectResult.Value, 
                    TimeSpan.FromSeconds(timeToLiveSeconds));

            }
        }

        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            StringBuilder keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}"); //Nối đối tượng vào chuỗi số
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}--{value}");

            }
            return keyBuilder.ToString();
        }
    }
}
