using AuthModule;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(AuthScheme.DefaultScheme)
        .AddScheme<AuthOption, AuthHandler>(AuthScheme.DefaultScheme, null);
var app = builder.Build();
app.UseWhen(
      predicate: x => x.Request.Path.StartsWithSegments(new PathString("/")),
      configuration: appBuilder => { appBuilder.UseAuth(); }
  );
app.MapGet("/", () => "Hello World!");

app.Run();
