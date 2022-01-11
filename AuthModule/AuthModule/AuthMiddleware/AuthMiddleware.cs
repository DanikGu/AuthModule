﻿using AuthModule.Model;
using Microsoft.AspNetCore.Authentication;

namespace AuthModule.AuthMiddleware
{

    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public AuthMiddleware(RequestDelegate next, ILoggerFactory LoggerFactory)
        {
            _next = next;
            _logger = LoggerFactory.CreateLogger<AuthMiddleware>();
        }
        public async Task Invoke(HttpContext httpContext, IAuthenticationService authenticationService, UsersDbContext usersDb)
        {
            var authenticated = await authenticationService.AuthenticateAsync(httpContext, AuthScheme.DefaultScheme);
            _logger.LogInformation("Access Status：" + authenticated.Succeeded);
            if (!authenticated.Succeeded)
            {
                await authenticationService.ChallengeAsync(httpContext, AuthScheme.DefaultScheme, new AuthenticationProperties());
                return;
            }
            await _next(httpContext);
        }
    }
}
