using Quizee.Modules.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quizee.Modules.UserPermissions.QueryModels
{
    public class GetAllUserQuery : UrlQueryModel
    {
        public List<string> RoleIds { get; set; }
    }
}
