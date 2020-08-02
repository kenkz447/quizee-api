using System.ComponentModel.DataAnnotations;
using Quizee.Modules.Models;

namespace Quizee.Modules.UserPermissions.Data.Models
{
    public class Department : ModelBase
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        public long? ParentId { get; set; }

        public virtual Department Parent { get; set; }
    }
}
