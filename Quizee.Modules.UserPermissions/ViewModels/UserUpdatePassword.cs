using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quizee.Modules.UserPermissions.ViewModels
{
    public class UserUpdatePassword
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
