using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quizee.Modules.UserPermissions.Data;
using Quizee.Modules.UserPermissions.Data.Models;
using Microsoft.EntityFrameworkCore;
using Quizee.Modules.Services;

namespace EASYSALON_SYSTEM_ADMIN_SERVICE.Implements
{
    public class DepartmentService: BaseService<UserPermissionsDbContext, Department>
    {
        public DepartmentService(
            UserPermissionsDbContext context) : base(context)
        {
        }

        public async Task<Department> CreateDepartment(Department department)
        {
            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();

            return department;
        }

        public async Task<Department> EditDepartment(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();

            return department;
        }

        public async Task<Department> DeleteDepartment(Department department)
        {
            _context.Entry(department).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return department;
        }

        public async Task<List<Department>> GetChilren(Department parent, List<Department> departments)
        {
            var children = _context.Departments
                .Where(x => x.ParentId == parent.Id)
                .Include(o => o.Parent)
                .ToList();

            foreach (var child in children)
            {
                departments.Add(child);
                departments = await GetChilren(child, departments);
            }

            return departments;
        }
    }
}
