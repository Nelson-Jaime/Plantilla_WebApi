using System;
using System.Collections.Generic;

namespace Plantilla.WebApi.Data.Contracts.Model
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
