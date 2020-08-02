using System;
using Microsoft.AspNetCore.Identity;
using Quizee.Modules.Models;

namespace Quizee.Modules.UserPermissions.Data.Models
{
    public class Role : IdentityRole, IModelBase
    {
        public string Description { get; set; }
        public long Level { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
    }
}
