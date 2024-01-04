using Domain.Exceptions;

namespace WebAPI.Middlewares
{
    public class AuthMiddleware : IMiddleware
    {
        private readonly IConfiguration _config;

        public AuthMiddleware(IConfiguration config)
        {
            _config = config;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string? providedKey = context.Request.Headers["X-Api-Key"];

            string? internalKey = _config.GetValue<string>("ApiKey") 
                ?? throw new ApiKeyNotFoundException("API KEY was not set.");

            if (providedKey is null || providedKey != internalKey)
                throw new IncorrectApiKeyException("Incorrect API KEY.");

            return next(context);
        }
    }
}
