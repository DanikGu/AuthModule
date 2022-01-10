using Microsoft.AspNetCore.Authentication;


namespace AuthModule
{
    public class AuthOption: AuthenticationSchemeOptions
    {
        public string? Realm { get; set; }
        public string? UserName { get; set; }
        public string? UserPwd { get; set; }
    }
}
