﻿using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectManagement
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Fullname { get; set; }
    }

    public class UserLoginDto
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class LoginFormModel
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }

    public class WebsiteAuthenticator : AuthenticationStateProvider
    {
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private readonly IConfiguration _configuration;

        public WebsiteAuthenticator(ProtectedLocalStorage protectedLocalStorage, IConfiguration configuration)
        {
            _protectedLocalStorage = protectedLocalStorage;
            _configuration = configuration;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var principal = new ClaimsPrincipal();

            try
            {
                var storedPrincipal = await _protectedLocalStorage.GetAsync<string>("identity");

                if (storedPrincipal.Success)
                {
                    //var userLoginDto = JsonConvert.DeserializeObject<UserLoginDto>(storedPrincipal.Value);
                    //var user = await new MyContext().Users
                    //    .FirstOrDefaultAsync(u => u.Username == userLoginDto.Username &&
                    //                              u.Password == userLoginDto.Password);
                    //var verificationResult = new PasswordHasher<object>().VerifyHashedPassword(null, user.Password, loginFormModel.Password);

                    var validateResult = ValidateCurrentToken(storedPrincipal.Value);

                    //if (user != null)
                    if (validateResult)
                    {
                        var userId = int.Parse(GetClaim(storedPrincipal.Value, "Id"));
                        var user = await new MyContext().Users.FirstOrDefaultAsync(x => x.Id == userId);
                        var identity = CreateIdentityFromUser(user);
                        principal = new(identity);
                    }
                }
            }
            catch
            {

            }

            return new AuthenticationState(principal);
        }

        public async Task LoginAsync(LoginFormModel loginFormModel)
        {
            var user = await new MyContext().Users
                .FirstOrDefaultAsync(u => u.Username == loginFormModel.Username /*&&
                                          u.Password == loginFormModel.Password*/);
            var verificationResult = new PasswordHasher<object>().VerifyHashedPassword(null, user.Password, loginFormModel.Password);

            var principal = new ClaimsPrincipal();

            if (verificationResult is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded)
            //if (user != null)
            {
                var identity = CreateIdentityFromUser(user);
                principal = new ClaimsPrincipal(identity);
                //var userLoginDto = new UserLoginDto { Username = user.Username, Password = GenerateToken(user.Id) };
                //await _protectedLocalStorage.SetAsync("identity", JsonConvert.SerializeObject(userLoginDto));
                await _protectedLocalStorage.SetAsync("identity", GenerateToken(user.Id));
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public async Task LogoutAsync()
        {
            await _protectedLocalStorage.DeleteAsync("identity");
            var principal = new ClaimsPrincipal();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        private static ClaimsIdentity CreateIdentityFromUser(User user)
        {
            return new ClaimsIdentity(new Claim[]
            {
            new (ClaimTypes.Name, user.Username),
            new (ClaimTypes.Hash, user.Password),
            new Claim("Id", $"{user.Id}"),
            new Claim("Fullname", user.Fullname)
            }, "username_password");
        }

        // https://dotnetcoretutorials.com/2020/01/15/creating-and-validating-jwt-tokens-in-asp-net-core/
        public string GenerateToken(int userId)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:JwtBearer:SecurityKey"]));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim("Id", userId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Authentication:JwtBearer:Issuer"],
                Audience = _configuration["Authentication:JwtBearer:Audience"],
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateCurrentToken(string token)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:JwtBearer:SecurityKey"]));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Authentication:JwtBearer:Issuer"],
                    ValidAudience = _configuration["Authentication:JwtBearer:Audience"],
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
            return stringClaimValue;
        }
    }
}
