using System;
using System.Linq;
using System.Threading.Tasks;
using Quizee.Modules.Auth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using Quizee.Modules.UserPermissions.Data.Models;
using Quizee.Modules.UserPermissions.Exts;
using Quizee.Modules.UserPermissions.ViewModels;
using Quizee.Utilities;

namespace Quizee.Modules.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : UserPermissionsControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;

        public AuthController(
           IConfiguration configuration,
           UserManager<User> userManager,
           SignInManager<User> signInManager) : base(userManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost("InitRootUser")]
        public async Task<ActionResult<User>> InitRootUser(UserVM body, string secrect)
        {
            if (secrect != "123")
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRootUser = _userManager.Users.FirstOrDefault(o => o.IsRoot == true);

            if (existingRootUser != null)
            {
                return BadRequest("ROOT_EXISTED");
            }

            var User = new User
            {
                UserName = body.UserName,
                Email = body.Email,
                IsRoot = true
            };

            var result = await _userManager.CreateAsync(User, body.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(User);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthLoginResVM>> Login(AuthLoginReqVM body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(body.Email);

            if (user == null)
            {
                return Unauthorized("USERNAME_OR_PASSWORD_INVALID");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, body.Password, false);
            if (signInResult.Succeeded == false)
            {
                return Unauthorized("USERNAME_OR_PASSWORD_INVALID");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var refreshToken = await _userManager.GetAuthenticationTokenAsync(user, "Default", "RefreshToken");
            if(refreshToken == null)
            {
                refreshToken = await _userManager.GenerateUserTokenAsync(user, "Default", "RefreshToken");
                var saveRefreshTokenResult = await _userManager.SetAuthenticationTokenAsync(user, "Default", "RefreshToken", refreshToken);

                if (!saveRefreshTokenResult.Succeeded)
                {
                    throw new Exception("SAVE_REFRESH_TOKEN_ERROR");
                }
            }

            var accessTokenId = Guid.NewGuid().ToString();

            var accessToken = JWTUtils.GenerateJWT(user, roles, _configuration["Jwt:Key"], _configuration["Jwt:Issuer"], accessTokenId);

            var response = new AuthLoginResVM
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = user
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<ActionResult<User>> ChangePassword(AuthChangePasswordVM body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identityResult = await _userManager.ChangePasswordAsync(
                CurrentUser,
                body.OldPassword,
                body.NewPassword);

            if (!identityResult.Succeeded)
            {
                return BadRequest(identityResult.Errors);
            }

            return Ok(User);
        }
    }
}