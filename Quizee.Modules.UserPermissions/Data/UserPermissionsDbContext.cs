using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Quizee.Modules.UserPermissions.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quizee.Modules.Models;

namespace Quizee.Modules.UserPermissions.Data
{
    public class UserPermissionsDbContext : IdentityDbContext<User, Role, string>
    {
        public UserPermissionsDbContext(
            DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; }
    }
}
