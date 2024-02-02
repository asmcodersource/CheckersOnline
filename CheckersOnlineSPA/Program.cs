using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CheckersOnlineSPA.ClientApp;
using Microsoft.EntityFrameworkCore;
using CheckersOnlineSPA.Data;
using CheckersOnlineSPA.Services.OnlineClientsWatcher;
using CheckersOnlineSPA.Services.Browser;
using Microsoft.AspNetCore.WebSockets;
using CheckersOnlineSPA.Services.Games;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5124");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddDataProtection();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<BrowserController>();
//builder.Services.AddSingleton<OnlineClientsWatcher>();
builder.Services.AddSingleton<GamesController>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Auth.AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = Auth.AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = Auth.AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });

string conn = "Server=192.168.0.101;Database=checkers;Uid=dimon;Pwd=qwerty123123;";
builder.Services.AddDbContext<DatabaseContext>(options => options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

var app = builder.Build();
app.UseCors(x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());
app.UseStaticFiles();
app.UseRouting();
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GameMiddleware>();
app.UseMiddleware<BrowserMiddleware>();
//app.UseMiddleware<ClientsCounterMiddleware>();


app.MapPost("/login", Auth.LoginHandler);
app.MapPost("/registration", Auth.RegistrationHandle);
app.MapPost("/tokenvalidation", Auth.TokenValidationHandler);
app.MapPost("/statistic", ClientsActivity.GetClientsActivityHandler );



app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapFallbackToFile("index.html");

app.Run();

