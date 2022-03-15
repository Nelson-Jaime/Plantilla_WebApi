using Plantilla.WebApi.Dto;
using System;

namespace Plantilla.WebApi.Data.Contracts.Model
{
    public class UserRoleDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        public virtual RoleDto Role { get; set; } = null!;
        public virtual UserDto User { get; set; } = null!;
    }
}
