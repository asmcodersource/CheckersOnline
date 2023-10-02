using CheckersOnlineSPA.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace CheckersOnlineSPA.ClientApp
{
    public static class Auth
    {
        public static class AuthOptions
        {
            public const string ISSUER = "MyAuthServer";
            public const string AUDIENCE = "MyAuthClient";
            const string KEY = "mysupersecret_secretkey!12312123456789012345678901234";
            public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }

        public static string PreparePassword(string password) => password;

        public static async Task LoginHandler(HttpContext context, DatabaseContext db )
        {
            JsonDocument document = await JsonDocument.ParseAsync(context.Request.Body);
            var email = document.RootElement.GetProperty("email").GetString();
            var password = document.RootElement.GetProperty("password").GetString();
            if (email is null || password is null)
            {
                await SendErrorMessage(context, 400, "Wrong request data format");
            }
            else
            {
                var preparedPassword = PreparePassword(password);
                var query = from u in db.Users
                            where u.Email == email && u.Password == preparedPassword
                            select u;
                if (query.Count() == 0)
                    await SendErrorMessage(context, 403, "Wrong login or password");
                else
                {
                    var user = query.First();
                    await AuthorizeClient(context, user);
                }
            }
        }

        private static async Task AuthorizeClient(HttpContext context, User user)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new { access_token = encodedJwt };
            await context.Response.WriteAsJsonAsync(response);
        }

        public static async Task SendErrorMessage( HttpContext context , int code, string message)
        {
            context.Response.StatusCode = code;
            await context.Response.WriteAsJsonAsync(new { errorMessage = message });
        }

        public static async Task RegistrationHandle(HttpContext context, DatabaseContext db)
        {
            JsonDocument document = await JsonDocument.ParseAsync(context.Request.Body);
            var email = document.RootElement.GetProperty("email").GetString();
            var username = document.RootElement.GetProperty("username").GetString();
            var password = document.RootElement.GetProperty("password").GetString();
            if (email is null || password is null || username is null )
                await SendErrorMessage(context, 400, "Wrong request data format");
            else
            {
                var query = from u in db.Users
                            where u.Email == email
                            select u;
                if (query.Count() != 0)
                {
                    await SendErrorMessage(context, 400, "That email is used!");
                }
                else
                {
                    var preparedPassword = PreparePassword(password);
                    User user = new User() { Email = email, Password = preparedPassword, UserName = username };
                    db.Add<User>(user);
                    var claims = new List<Claim> { new Claim(ClaimTypes.Email, user.Email), new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };
                    var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.ISSUER,
                            audience: AuthOptions.AUDIENCE,
                            claims: claims,
                            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                    var response = new { access_token = encodedJwt };
                    await context.Response.WriteAsJsonAsync(response);
                    await db.SaveChangesAsync();
                }
            }
        }

        [Authorize]
        public static async Task TokenValidationHandler(HttpContext context, DatabaseContext db)
        {
            var email = context.User.Claims.First( (claim) => claim.Type == ClaimTypes.Email).Value;
            var query = from u in db.Users
                        where u.Email == email
                        select u;
            if (query.Count() == 0)
            {
                await SendErrorMessage(context, 403, "Token has wrong claims");
                return;
            }
            var user = query.First();
            var response =  new { user.Id, user.UserName, user.Email, user.Picture };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
