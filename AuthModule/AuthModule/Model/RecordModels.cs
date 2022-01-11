namespace AuthModule.Model
{
    public record LoginUser(string UserName, string Password);
    public record SignUpUser(string UserName, string Password, string SecondPassword);
}
