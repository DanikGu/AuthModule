using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace AuthModule
{
    public class AuthHandler : AuthenticationHandler<AuthOption>
    {
        private readonly AuthOption authOptions;
        public AuthHandler(IOptionsMonitor<AuthOption> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
          : base(options, logger, encoder, clock)
        {
            authOptions = options.CurrentValue;
        }

        /// <summary>
        ///Certification
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");
            string username, password;
            try
            {
                AuthenticationHeaderValue authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader?.Parameter ?? string.Empty);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                username = credentials[0];
                password = credentials[1];
                var isValidUser = IsAuthorized(username, password);
                if (isValidUser == false)
                {
                    return AuthenticateResult.Fail("Invalid username or password");
                }
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier,username),
                new Claim(ClaimTypes.Name,username),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }
        /// <summary>
        ///Questions
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\"";
            await base.HandleChallengeAsync(properties);
        }

        /// <summary>
        ///Certification Fail
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            await base.HandleForbiddenAsync(properties);
        }

        private bool IsAuthorized(string username, string password)
        {
            return username.Equals("Danik")
                && password.Equals("Danik");
        }
    }
    public static class AuthScheme
    {
        public const string DefaultScheme = "Basic";
    }
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public AuthMiddleware(RequestDelegate next, ILoggerFactory LoggerFactory)
        {
            _next = next;
            _logger = LoggerFactory.CreateLogger<AuthMiddleware>();
        }
        public async Task Invoke(HttpContext httpContext, IAuthenticationService authenticationService)
        {
            var authenticated = await authenticationService.AuthenticateAsync(httpContext, AuthScheme.DefaultScheme);
            _logger.LogInformation("Access Status：" + authenticated.Succeeded);
            if (!authenticated.Succeeded)
            {
                await authenticationService.ChallengeAsync(httpContext, AuthScheme.DefaultScheme, new AuthenticationProperties { });
                return;
            }
            await _next(httpContext);
        }
    }
    public static class AppBuilderExstensionClass{
        public static void UseAuth(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuthMiddleware>();
        }
    }
}
