using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using AuthModule.AuthMiddleware;
using AuthModule.Model;
using AuthModule.Helpers;
using AuthModule.Extensions;

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
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public JsonResult Login(LoginUser loginUser)
        {
            User user = _context.User.FirstOrDefault(user => user.UserName == loginUser.UserName && user.Password == CreateHashString.GetHashString(loginUser.Password ?? ""));
            if (user == null) {
                return new JsonResult(new { Succsess = false, Message = "User not found" } );
            }
            else {
                user.IsAuthenticated = true;
                HttpContext.Session.Set("User", user);
                return new JsonResult(new { Succsess = true });
            }
        }
        [AllowAnonymous]
        public JsonResult SignUp(SignUpUser signUpUser) {
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
            HttpContext.Session.Set("User", newUser);
            _context.User?.Add(newUser);
            _context.SaveChangesAsync();
            return new JsonResult(new { Succsess = true });

        }
        
        public ActionResult Logout() {
            return View();
        }
        public record LoginUser(string UserName, string Password);
        public record SignUpUser(string UserName, string Password, string SecondPassword);
    }
}
