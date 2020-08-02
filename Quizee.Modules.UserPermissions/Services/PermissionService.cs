using Quizee.Modules.UserPermissions.Exts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Quizee.Modules.UserPermissions.Services
{
    public class PermissionService
    {
        private readonly PermissionRequirementCollector _permissionRequirementCollector;

        public PermissionService()
        {
            var asm = Assembly.GetExecutingAssembly();
            _permissionRequirementCollector = new PermissionRequirementCollector(asm);
        }

        public IEnumerable<PermissionRequirement> GetAllPermissions()
        {
            return _permissionRequirementCollector.GetPermissionRequirements();
        }
    }
}
