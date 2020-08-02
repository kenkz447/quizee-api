using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Quizee.Modules.UserPermissions.Data.Models;

namespace Quizee.Modules.UserPermissions.ViewModels
{
    public class UserVM : User
    {
        public UserVM()
        {

        }

        public UserVM(User applicationUser)
        {
            this.CreatedAt = applicationUser.CreatedAt;
            this.CreatedBy = applicationUser.CreatedBy;
            this.DeletedAt = applicationUser.DeletedAt;
            this.DeletedBy = applicationUser.DeletedBy;
            this.Department = applicationUser.Department;
            this.DepartmentId = applicationUser.DepartmentId;
            this.Email = applicationUser.Email;
            this.EmailConfirmed = applicationUser.EmailConfirmed;
            this.Id = applicationUser.Id;
            this.IsRoot = applicationUser.IsRoot;
            this.LockoutEnabled = applicationUser.LockoutEnabled;
            this.LockoutEnd = applicationUser.LockoutEnd;
            this.NormalizedEmail = applicationUser.NormalizedEmail;
            this.NormalizedUserName = applicationUser.NormalizedUserName;
            this.PhoneNumber = applicationUser.PhoneNumber;
            this.PhoneNumberConfirmed = applicationUser.PhoneNumberConfirmed;
            this.UpdateBy = applicationUser.UpdateBy;
            this.UpdatedAt = applicationUser.UpdatedAt;
            this.UserName = applicationUser.UserName;
        }

        [Required]
        public string Password { get; set; }

        [Required]
        public List<string> RoleIds { get; set; }

        public IEnumerable<Role> Roles { get; set; }
    }
}
