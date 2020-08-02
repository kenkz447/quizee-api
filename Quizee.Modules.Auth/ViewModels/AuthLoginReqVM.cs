using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quizee.Modules.Auth.ViewModels
{
    public class AuthLoginReqVM
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
