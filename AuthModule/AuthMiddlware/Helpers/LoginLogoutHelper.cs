using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthMiddlware.Model;
using AuthMiddlware.Extensions;
using Microsoft.AspNetCore.Http;

namespace AuthMiddlware.Helpers
{
    public class LoginLogoutHelper
    {
        public void Login(User user, HttpContext httpContext) {
            if (httpContext.Session == null) { throw new Exception("Session is null"); }
            httpContext.Session.Set("User", user);
        }
        public void Logout(HttpContext httpContext) {
            if (httpContext.Session == null) { throw new Exception("Session is null"); }
            httpContext.Session.Set("User", new User());
        }
    }
}
