using Microsoft.AspNetCore.Builder;

namespace AuthModule.Extensions
{
    public static class AppBuilderExstension
    {
        public static void UseAuth(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuthMiddleware.AuthMiddleware>();
        }
    }
}
