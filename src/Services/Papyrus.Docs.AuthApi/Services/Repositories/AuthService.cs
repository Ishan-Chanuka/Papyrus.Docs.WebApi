using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Papyrus.Docs.AuthApi.Data;
using Papyrus.Docs.AuthApi.Models.DbModels;
using Papyrus.Docs.AuthApi.Primitives.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Papyrus.Docs.AuthApi.Services.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
        }

        #region Private methods
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiSettings:SecretKey"]!)),
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

        private async Task<JwtSecurityToken> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? ""),
                new(ClaimTypes.Email, user.Email ?? "")
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApiSettings:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["ApiSettings:Issuer"],
                audience: _configuration["ApiSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return token;
        }

        #endregion

        public async Task<ApiResponse<bool>> RegisterAsync(RegisterDto request)
        {
            string message;
            if (request == null)
            {
                message = "Invalid request";
                return new ApiResponse<bool>(message: message, (int)HttpStatusCode.BadRequest); // 400
            }

            var applicationUser = await _userManager.FindByEmailAsync(request.Email);

            if (applicationUser != null)
            {
                message = "User already exists";
                return new ApiResponse<bool>(message: message, (int)HttpStatusCode.BadRequest); // 400
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                NormalizedEmail = request.Email.ToUpper(),
                EmailConfirmed = true
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    message = "User creation failed! Please check user details and try again.";
                    return new ApiResponse<bool>(message: message, (int)HttpStatusCode.BadRequest); // 400
                }

                var role = await _roleManager.FindByNameAsync(request.Role)
                                ?? throw new ApiException(message: "Role not found", statusCode: (int)HttpStatusCode.NotFound); // 404

                var roleResult = await _userManager.AddToRoleAsync(user, role.Name);

                if (!roleResult.Succeeded)
                {
                    message = "User creation failed! Please check user details and try again.";
                    return new ApiResponse<bool>(message: message, (int)HttpStatusCode.BadRequest); // 400
                }

                await transaction.CommitAsync();

                return new ApiResponse<bool>
                {
                    StatusCode = (int)HttpStatusCode.OK, // 200
                    Message = "User created successfully",
                    IsSuccess = true,
                    Data = true
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                return new ApiResponse<bool>
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError, // 500
                    Message = "An error occurred. Please try again later.",
                    IsSuccess = false,
                    Data = false
                };
            }
        }

        public async Task<ApiResponse<AuthResponseModel>> LoginAsync(LoginDto request)
        {
            string message;
            if (request == null)
            {
                message = "Invalid request";
                return new ApiResponse<AuthResponseModel>(message: message, (int)HttpStatusCode.BadRequest); // 400
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                message = "User not found";
                return new ApiResponse<AuthResponseModel>(message: message, (int)HttpStatusCode.NotFound); // 404
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
            {
                message = "Login faild";
                return new ApiResponse<AuthResponseModel>(message: message, (int)HttpStatusCode.BadRequest); // 400
            }

            var token = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(30);
            await _userManager.UpdateAsync(user);

            var response = new AuthResponseModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken
            };

            return new ApiResponse<AuthResponseModel>
            {
                StatusCode = (int)HttpStatusCode.OK, // 200
                Message = "Login successful",
                IsSuccess = true,
                Data = response
            };
        }

        public async Task<ApiResponse<AuthResponseModel>> RefreshTokenAsync(TokenDto request)
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            var username = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return new ApiResponse<AuthResponseModel>(message: "Unauthorize", statusCode: (int)HttpStatusCode.Unauthorized);
            }

            var newToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(30);
            await _userManager.UpdateAsync(user);

            var response = new AuthResponseModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(newToken),
                RefreshToken = newRefreshToken
            };

            return new ApiResponse<AuthResponseModel>
            {
                StatusCode = (int)HttpStatusCode.OK, // 200
                Message = "Login successful",
                IsSuccess = true,
                Data = response
            };
        }

    }
}
