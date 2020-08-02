using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Quizee.Modules.UserPermissions.Exts
{
    public class PermissionRequirementCollector
    {
        private readonly Assembly _assembly;
        private List<string> _policies;

        public PermissionRequirementCollector(Assembly assembly)
        {
            _assembly = assembly;
        }

        private List<string> GetAuthorizePolicies()
        {
            if(_policies == null)
            {
                _policies = _assembly.GetTypes()
                   .Where(type => typeof(UserPermissionsControllerBase).IsAssignableFrom(type))
                   .SelectMany(type => type.GetMethods())
                   .Where(method => method.IsPublic && method.IsDefined(typeof(AuthorizeAttribute)))
                   .Select(method => method.GetCustomAttribute<AuthorizeAttribute>())
                   .Where(attribule => attribule.Policy != null)
                   .Select(attribule => attribule.Policy)
                   .ToList();
            }

            return _policies;
        }

        public List<PermissionRequirement> GetPermissionRequirements()
        {
            var result = new List<PermissionRequirement>();
            var policies = GetAuthorizePolicies();

            policies.ForEach(policy =>
            {
                result.Add(new PermissionRequirement(policy));
            });

            return result;
        }
    }
}
