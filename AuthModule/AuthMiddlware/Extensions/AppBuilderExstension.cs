using Microsoft.AspNetCore.Builder;
using AuthMiddlware.AuthMiddleware;
namespace AuthMiddlware.Extensions
{
    public static class AppBuilderExstension
    {
        public static void UseAuth(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuthMiddleware.AuthMiddleware>();
        }
    }
}
