using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Quizee.Modules.UserPermissions.Exts;
using Quizee.Modules.UserPermissions.Services;
using Quizee.Modules.UserPermissions.ViewModels;
using Quizee.Modules.UserPermissions.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizee.Modules.QueryModels;
using Quizee.Modules.ViewModels;

namespace Quizee.Modules.UserPermissions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RolesController : UserPermissionsControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly PermissionService _permissionService;

        public RolesController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            PermissionService permissionService
            ) : base(userManager)
        {
            _roleManager = roleManager;
            _permissionService = permissionService;
        }

        [HttpGet("My")]
        public async Task<ActionResult<List<RoleVM>>> My()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(this.User);

            if (user.IsRoot)
            {
                var allPermissions = _permissionService.GetAllPermissions();
                var rootRole = new RoleVM()
                {
                    Name = "root",
                    Id = "root",
                    IdentityRoleClaims = allPermissions
                    .Select(permission =>
                        {
                            return new IdentityRoleClaim<long>
                            {
                                ClaimType = CustomClaimTypes.Permission,
                                ClaimValue = permission.Policy
                            };
                        })
                        .ToList()
                };

                return Ok(new List<RoleVM>() { rootRole });
            }

            var roleNames = await _userManager.GetRolesAsync(user);

            var roles = _roleManager.Roles.Where(role => roleNames.Contains(role.Name)).ToList();

            var buildRoleClaimsTasks = roles.Select(async (role) =>
            {
                var applicationRoleVM = new RoleVM(role);
                var claims = await _roleManager.GetClaimsAsync(role);

                applicationRoleVM.IdentityRoleClaims = claims
                    .Select(claim =>
                    {
                        var identityRoleClaim = new IdentityRoleClaim<long>();
                        identityRoleClaim.InitializeFromClaim(claim);
                        return identityRoleClaim;
                    })
                    .ToList();

                return applicationRoleVM;
            });

            var result = await Task.WhenAll(buildRoleClaimsTasks);

            return Ok(result);
        }

        [Authorize("Roles/GetAll")]
        [HttpGet]
        public async Task<ActionResult<ResultList<Role>>> GetAll([FromQuery] UrlQueryModel query)
        {
            var roles = _roleManager.Roles;

            return OkPagination(roles, query);
        }

        [Authorize("Roles/GetById")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleVM>> GetById(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound("ENTRY_NOT_FOUND");
            }

            var claims = await _roleManager.GetClaimsAsync(role);

            var result = new RoleVM(role);

            result.IdentityRoleClaims = claims
                .Select(claim =>
                {
                    var roleClaim = new IdentityRoleClaim<long>();
                    roleClaim.InitializeFromClaim(claim);

                    return roleClaim;
                })
                .ToList();

            return Ok(result);
        }

        [Authorize("Roles/Create")]
        [HttpPost("Create")]
        public async Task<ActionResult<Role>> Create(Role body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new Role();

            role.Name = body.Name;
            role.Description = body.Description;
            role.Level = body.Level;
            role.CreatedAt = DateTime.Now;
            role.CreatedBy = CurrentUser.Email;

            var roleAction = await _roleManager.CreateAsync(role);

            if (roleAction == null)
            {
                return BadRequest("BadRequest");
            }

            if (!roleAction.Succeeded)
            {
                return BadRequest(roleAction.Errors);
            }

            return Ok(role);
        }

        [Authorize("Roles/UpdateRoleClaims")]
        [HttpPost("UpdateRoleClaims")]
        public async Task<ActionResult<RoleVM>> UpdateRoleClaims(RoleVM body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _roleManager.Roles.Where(e => e.Id == body.Id).FirstAsync();

            if (role == null)
            {
                return NotFound("ENTRY_NOT_FOUND");
            }

            var currentRoleClaims = await _roleManager.GetClaimsAsync(role);

            foreach (var claim in body.IdentityRoleClaims)
            {
                if (currentRoleClaims.FirstOrDefault(o => o.Value == claim.ClaimValue) == null)
                {
                    await _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, claim.ClaimValue));
                }
            }

            foreach (var item in currentRoleClaims)
            {
                if (body.IdentityRoleClaims.FirstOrDefault(o => o.ClaimValue == item.Value) == null)
                {
                    await _roleManager.RemoveClaimAsync(role, item);
                }
            }

            return Ok(role);
        }

        [Authorize("Roles/Update")]
        [HttpPut]
        public async Task<ActionResult<Role>> Update(Role body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _roleManager.FindByIdAsync(body.Id);
            if (role == null)
            {
                return NotFound("ENTRY_NOT_FOUND");
            }

            role.Name = body.Name;
            role.Description = body.Description;
            role.Level = body.Level;
            role.UpdatedAt = DateTime.Now;
            role.UpdateBy = CurrentUser.Email;

            var result = await _roleManager.UpdateAsync(role);

            return Ok(result);
        }

        [Authorize("Roles/Delete")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Role>> Delete(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound("ENTRY_NOT_FOUND");
            }

            role.DeletedAt = DateTime.Now;
            role.DeletedBy = CurrentUser.Email;

            var result = await _roleManager.UpdateAsync(role);

            return Ok(result);
        }
    }
}
