using AuthMiddlware.Model;
using System.Security.Claims;
using System.Text.Encodings.Web;
using AuthMiddlware.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace AuthMiddlware.AuthMiddleware
{
    public class AuthHandler : AuthenticationHandler<AuthOption>
    {
        private readonly AuthOption authOptions;
        public AuthHandler(IOptionsMonitor<AuthOption> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
          : base(options, logger, encoder, clock)
        {
            authOptions = options.CurrentValue;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (GetUser().IsAuthenticated) {
                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, GetUser().UserName ?? throw new Exception("Username empty")),
                    new Claim(ClaimTypes.Name, GetUser().UserName ?? throw new Exception("Username empty")),
                    new Claim(ClaimTypes.Role, "User")
                };
                var identity = new ClaimsIdentity(claims, AuthScheme.DefaultScheme);
                var principal = new ClaimsPrincipal(identity);
                Request.HttpContext.User = principal;
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return await Task.FromResult(AuthenticateResult.Success(ticket));
            }
            var claimsAnoymys = new[] {
                new Claim(ClaimTypes.Role, ClaimTypes.Anonymous)
            };
            var identityAnoymys = new ClaimsIdentity(claimsAnoymys, Scheme.Name);
            var principalAnoymys = new ClaimsPrincipal(identityAnoymys);
            var ticketAnoymys = new AuthenticationTicket(principalAnoymys, Scheme.Name);
            return await Task.FromResult(AuthenticateResult.Success(ticketAnoymys));
        }
        private User GetUser() {
            return Request.HttpContext.Session.Get<User>("User") ?? new User();
        } 
    }


}
