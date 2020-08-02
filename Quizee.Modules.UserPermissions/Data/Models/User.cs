using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Quizee.Modules.Models;

namespace Quizee.Modules.UserPermissions.Data.Models
{
    public class User : IdentityUser, IModelBase
    {
        public bool IsRoot { get; set; }
        public long? DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }
    }
}
