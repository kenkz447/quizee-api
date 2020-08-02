using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quizee.Modules.UserPermissions.Data.Models;

namespace Quizee.Modules.UserPermissions.ViewModels
{
    public class RoleVM : Role
    {
        public RoleVM()
        {

        }

        public RoleVM(Role role)
        {
            this.Id = role.Id;
            this.ConcurrencyStamp = role.ConcurrencyStamp;
            this.Name = role.Name;
            this.NormalizedName = role.NormalizedName;
            this.Description = role.Description;
            this.Level = role.Level;

            this.CreatedBy = role.CreatedBy;
            this.CreatedAt = role.CreatedAt;
            this.UpdatedAt = role.UpdatedAt;
            this.UpdateBy = role.UpdateBy;
            this.DeletedAt = role.DeletedAt;
            this.DeletedBy = role.DeletedBy;
        }

        public List<IdentityRoleClaim<long>> IdentityRoleClaims { get; set; }
    }
}
