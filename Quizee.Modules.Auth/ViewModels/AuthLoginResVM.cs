using Quizee.Modules.UserPermissions.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quizee.Modules.Auth.ViewModels
{
    public class AuthLoginResVM
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public User User { get; set; }
    }
}
