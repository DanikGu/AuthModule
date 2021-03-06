using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using AuthMiddlware.AuthMiddleware;
using AuthMiddlware.Model;
using AuthMiddlware.Extensions;
using AuthMiddlware.Helpers;
using AuthModule.Model;
using AuthModule.Helpers;

namespace AuthModule.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        private readonly UsersDbContext _context;

        public LoginController(UsersDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Login(LoginUser loginUser, string redirectUrl = "/")
        {
            User? user = _context?.User?.FirstOrDefault(user => user.UserName == loginUser.UserName && user.Password == CreateHashString.GetHashString(loginUser.Password ?? ""));
            if (user == null) {
                return new JsonResult(new { Succsess = false, Message = "User not found" } );
            }
            else {
                user.IsAuthenticated = true;
                new LoginLogoutHelper().Login(user, HttpContext);
                Response.Redirect(redirectUrl);
                return new JsonResult(new { Succsess = true });
            }
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult SignUp(SignUpUser signUpUser, string redirectUrl = "/") {
            if (signUpUser == null) {
                return new JsonResult(new { Succsess = false, Message = "User data are empty" });
            }
            if (signUpUser.Password != signUpUser.SecondPassword) {
                return new JsonResult(new { Succsess = false, Message = "Password are not the same" });
            }
            User newUser = new User()
            {
                IsAuthenticated = true,
                UserName = signUpUser.UserName,
                Password = CreateHashString.GetHashString(signUpUser.Password)
            };
            new LoginLogoutHelper().Login(newUser, HttpContext);
            _context.User?.Add(newUser);
            _context.SaveChangesAsync();
            Response.Redirect(redirectUrl);
            return new JsonResult(new { Succsess = true });
        }
        [Authorize(Roles = "User")]
        public void Logout(string redirectUrl = "/") {
            new LoginLogoutHelper().Logout(HttpContext);
            Response.Redirect(redirectUrl);
        }
    }
}
