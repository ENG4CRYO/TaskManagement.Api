using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Application.Interfaces;
using TaskManagement.Core.Entities;

namespace TaskManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jWT)
        {
            _userManager = userManager;
            _jwt = jWT.Value;
        }
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel { Message = "Email is already registered!" };
            }

            if (await _userManager.FindByNameAsync(model.UserName) is not null)
            {
                return new AuthModel { Message = "Username is already registered!" };
            }


            var user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }

                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "User");
            var token = await CreateJwtToken(user);

            return new AuthModel
            {
                UserName = user.UserName,
                Email = user.Email,
                ExpiresOn = token.ValidTo,
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Roles = new List<string> { "User" },
                Message = "User registered successfully!"
            };

        }

        public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }.Union(userClaims).Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.Duration),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10), 
                CreatedOn = DateTime.UtcNow
            };
        }


        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

 
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

           
            var jwtSecurityToken = await CreateJwtToken(user);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var refreshToken = GenerateRefreshToken();

            if (user.RefreshTokens == null)
                user.RefreshTokens = new List<RefreshToken>();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user); 
      
            var roleList = await _userManager.GetRolesAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Token = tokenString,
                RefreshToken = refreshToken.Token, 
                RefreshTokenExpiration = refreshToken.ExpiresOn,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roleList.ToList(),
                ExpiresOn = jwtSecurityToken.ValidTo
            };
        }

        public async Task<AuthModel> RefreshTokenAsync(string token, string refreshToken)
        {
            var authModel = new AuthModel();


            var principal = GetPrincipalFromExpiredToken(token);
            if (principal == null)
            {
                authModel.Message = "Invalid Token";
                return authModel;
            }

            var userId = principal.Claims.SingleOrDefault(c => c.Type == "uid")?.Value;
            var user = await _userManager.Users
    .Include(u => u.RefreshTokens)
    .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.RefreshTokens == null)
            {
                authModel.Message = "Invalid Token";
                return authModel;
            }

     
            var storedRefreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken);

        
            if (storedRefreshToken == null || !storedRefreshToken.IsActive)
            {
                authModel.Message = "Invalid Refresh Token";
                return authModel;
            }

      
            storedRefreshToken.RevokedOn = DateTime.UtcNow;

            var newJwtToken = await CreateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(newJwtToken),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                UserName = user.UserName,
                Email = user.Email,
                ExpiresOn = newJwtToken.ValidTo
            };
        }

     
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true, 
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
                ValidIssuer = _jwt.Issuer,
                ValidAudience = _jwt.Audience,
                ValidateLifetime = false 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }




    }
}
