
using Quizee.Modules.UserPermissions.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Quizee.Modules.UserPermissions.Exts
{
    public abstract class UserPermissionsControllerBase : QuizeeModulesControllerBase
    {
        private User _applicationUser;

        public readonly UserManager<User> _userManager;

        protected UserPermissionsControllerBase(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public User CurrentUser
        {
            get
            {
                if (_applicationUser != null)
                {
                    return _applicationUser;
                }

                var user = _userManager.GetUserAsync(this.User);

                _applicationUser = user.Result;

                return _applicationUser;
            }
        }
    }
}
