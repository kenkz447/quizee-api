using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quizee.Modules.UserPermissions.Exts;
using Quizee.Modules.UserPermissions.Data.Models;
using EASYSALON_SYSTEM_ADMIN_SERVICE.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quizee.Modules.ViewModels;
using Quizee.Modules.QueryModels;

namespace Quizee.Modules.UserPermissions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : UserPermissionsControllerBase
    {
        private readonly DepartmentService _departmentService;

        public DepartmentsController(
            UserManager<User> userManager,
            DepartmentService departmentService) : base(userManager)
        {
            _departmentService = departmentService;
        }

        [Authorize("Departments/GetAll")]
        [HttpGet]
        public async Task<ActionResult<ResultList<Department>>> GetAll([FromQuery] UrlQueryModel query)
        {
            if (CurrentUser.IsRoot)
            {
                var allDepartments = _departmentService.DbSet
                    .Include(o => o.Parent);

                return OkPagination(allDepartments, query);
            }

            var myDepartment = _departmentService.DbSet
                .Where(x => x.Id == CurrentUser.DepartmentId)
                .Include(o => o.Parent)
                .FirstOrDefault();

            if (myDepartment == null)
            {
                return OkPagination<Department>(null, query);
            }

            var departments = new List<Department>
            {
                myDepartment
            };

            var result = await _departmentService.GetChilren(myDepartment, departments);

            return OkPagination(result, query);
        }

        [Authorize("Departments/GetById")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetById(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var department = await _departmentService.DbSet.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        [Authorize("Departments/Create")]
        [HttpPost]
        public async Task<ActionResult<Department>> Create(Department body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            body.CreatedAt = DateTime.Now;
            body.CreatedBy = CurrentUser.Email;

            await _departmentService.CreateDepartment(body);

            return Ok(body);
        }

        [Authorize("Departments/Update")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Department>> Update(long id, Department body)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == body.ParentId)
            {
                return NotFound("ENTRY_NOT_FOUND");
            }

            var existedDepartment = await _departmentService.DbSet.FindAsync(id);

            if (existedDepartment == null)
            {
                return NotFound("ENTRY_NOT_FOUND");
            }

            existedDepartment.Address = existedDepartment.Address;
            existedDepartment.Name = existedDepartment.Name;

            existedDepartment.UpdatedAt = DateTime.Now;
            existedDepartment.UpdateBy = CurrentUser.Email;

            existedDepartment.ParentId = body.ParentId;
            await _departmentService.EditDepartment(existedDepartment);

            return Ok(existedDepartment);
        }

        [Authorize("Departments/Delete")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> Delete(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existedDepartment = await _departmentService.DbSet.FindAsync(id);

            if (existedDepartment == null)
            {
                return NotFound("ENTRY_NOT_FOUND");
            }

            existedDepartment.DeletedAt = DateTime.Now;
            existedDepartment.DeletedBy = CurrentUser.Email;

            await _departmentService.DeleteDepartment(existedDepartment);

            return Ok(existedDepartment);
        }
    }
}
