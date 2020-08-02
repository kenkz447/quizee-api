using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quizee.Modules.UserPermissions.Exts;
using Quizee.Modules.UserPermissions.ViewModels;
using Quizee.Modules.UserPermissions.Data;
using Quizee.Modules.UserPermissions.Data.Models;
using EASYSALON_SYSTEM_ADMIN_SERVICE.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizee.Modules.ViewModels;
using Quizee.Modules.UserPermissions.QueryModels;

namespace Quizee.Modules.UserPermissions.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : UserPermissionsControllerBase
    {
        private readonly DepartmentService _departmentService;

        private readonly RoleManager<Role> _roleManager;
        private readonly UserPermissionsDbContext _dbcontext;
        public UsersController(
            DepartmentService departmentService,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            UserPermissionsDbContext dbcontext) : base(userManager)
        {
            this._departmentService = departmentService;
            this._roleManager = roleManager;
            _dbcontext = dbcontext;
        }

        [HttpGet("Me")]
        public async Task<ActionResult<User>> Me()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var me = await _userManager.GetUserAsync(User);
            if (me == null)
            {
                return NotFound("ENTRY_NOT_FOUND");
            }

            return Ok(me);
        }

        [Authorize("Users/GetAll")]
        [HttpGet]
        public async Task<ActionResult<ResultList<UserVM>>> GetAll([FromQuery] GetAllUserQuery query)
        {
            var childDepartments = new List<Department>();

            if (CurrentUser.IsRoot)
            {
                childDepartments = _departmentService.DbSet.ToList();
            }
            else
            {
                var myDepartment = _departmentService.DbSet
                   .Where(x => x.Id == CurrentUser.DepartmentId)
                   .FirstOrDefault();

                if (myDepartment == null)
                {
                    return OkPagination<UserVM>(null, query);
                }

                var departments = new List<Department>
                {
                    myDepartment
                };

                childDepartments = await _departmentService.GetChilren(myDepartment, departments);
            }

            var allRoles = await _roleManager.Roles.ToListAsync();

            var userVMs = _userManager.Users
                .Where(user => childDepartments.Contains(user.Department))
                .Include(o => o.Department)
                .ToList()
                .Select(user =>
                {
                    var userVM = new UserVM(user);
                    var userRoleNames = _userManager.GetRolesAsync(user).Result;

                    var roles = allRoles.Where(role => userRoleNames.Contains(role.Name));
                    userVM.RoleIds = roles.Select(role => role.Id).ToList();
                    userVM.Roles = roles;

                    return userVM;
                });

            return OkPagination(userVMs, query);
        }

        [Authorize("Users/Create")]
        [HttpPost]
        public async Task<ActionResult<User>> Create(UserVM body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applicationUserInit = new User
            {
                UserName = body.UserName,
                Email = body.Email,
                IsRoot = false,
                DepartmentId = body.DepartmentId,
                CreatedAt = DateTime.Now,
                CreatedBy = CurrentUser.Email
            };

            var roles = _roleManager.Roles.Where(role => body.RoleIds.Contains(role.Id));
            var roleNames = roles.Select(role => role.Name).ToList();

            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var createUserResult = await _userManager.CreateAsync(applicationUserInit, body.Password);
                    if (!createUserResult.Succeeded)
                    {
                        return BadRequest(createUserResult.Errors);
                    }

                    var addToRolesResult = await _userManager.AddToRolesAsync(applicationUserInit, roleNames);
                    if (!addToRolesResult.Succeeded)
                    {
                        return BadRequest(createUserResult.Errors);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    transaction.Dispose();
                }
            }

            return Ok(body);
        }

        [Authorize("Users/Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UserVM body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("ENTRY_NOT_FOUND");
            }

            user.UserName = body.UserName;
            user.Email = body.Email;
            user.DepartmentId = body.DepartmentId;

            user.UpdatedAt = DateTime.Now;
            user.UpdateBy = CurrentUser.Email;

            var userRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = _roleManager.Roles.Where(role => userRoles.Contains(role.Name));
            var romeNamesToRemove = rolesToRemove.Select(role => role.Name).ToList();

            var rolesToAdd = _roleManager.Roles.Where(role => body.RoleIds.Contains(role.Id));
            var roleNamesToAdd = rolesToAdd.Select(role => role.Name).ToList();

            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        return BadRequest(updateResult.Errors);
                    }

                    var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, romeNamesToRemove);
                    if (!removeRolesResult.Succeeded)
                    {
                        return BadRequest(removeRolesResult.Errors);
                    }

                    var addRolesResult = await _userManager.AddToRolesAsync(user, roleNamesToAdd);
                    if (!addRolesResult.Succeeded)
                    {
                        return BadRequest(addRolesResult.Errors);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    transaction.Dispose();
                }
            }

            return Ok(user);
        }

        [Authorize("Users/ChangePassword")]
        [HttpPut("ChangePassword/{id}")]
        public async Task<IActionResult> UpdatePassword(string id, UserUpdatePassword body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return BadRequest("ENTRY_NOT_FOUND");
            }

            foreach (var validator in _userManager.PasswordValidators)
            {
                var validateResult = await validator.ValidateAsync(_userManager, user, body.NewPassword);
                if(!validateResult.Succeeded)
                {
                    return BadRequest(validateResult.Errors);
                }
            }

            var newPassword = _userManager.PasswordHasher.HashPassword(user, body.NewPassword);

            user.PasswordHash = newPassword;
            user.UpdatedAt = DateTime.Now;
            user.UpdateBy = CurrentUser.Email;

            using (var transaction = _dbcontext.Database.BeginTransaction())
            {
                try
                {
                    var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                    if(!removePasswordResult.Succeeded)
                    {
                        throw new Exception("Error occurred: RemovePasswordAsync");
                    }

                    var AddPasswordResult = await _userManager.AddPasswordAsync(user, body.NewPassword);
                    if (!AddPasswordResult.Succeeded)
                    {
                        throw new Exception("Error occurred: AddPasswordAsync");
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                } finally
                {
                    transaction.Dispose();
                }
            }

            user.PasswordHash = null;

            return Ok(user);
        }

        [Authorize("Users/Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return BadRequest("ENTRY_NOT_FOUND");
            }

            user.DeletedAt = DateTime.Now;
            user.DeletedBy = CurrentUser.Email;

            var identityResult = await _userManager.UpdateAsync(user);

            if (!identityResult.Succeeded)
            {
                return BadRequest(identityResult.Errors);
            }

            return Ok(user);
        }
    }
}
