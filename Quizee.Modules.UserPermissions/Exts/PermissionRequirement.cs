using System;
using Microsoft.AspNetCore.Authorization;
using PluralizeService.Core;

using static Quizee.Utilities.StringUtils;

namespace Quizee.Modules.UserPermissions.Exts
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        private string _name;
        public string Policy { get; private set; }
        public string Name
        {
            get
            {
                if (_name != null)
                {
                    return _name;
                }

                var permissionParts = Policy.Split('/');
                var permissionScope = permissionParts[0];

                var name = FirstLetterToUpper(PluralizationProvider.Pluralize(permissionScope));

                for (int i = 1; i < permissionParts.Length; i++)
                {
                    name += FirstLetterToUpper(permissionParts[i]);
                }

                return _name = name;
            }
        }

        public PermissionRequirement(string permission)
        {
            Policy = permission;
        }
    }
}
