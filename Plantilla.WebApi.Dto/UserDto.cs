using Microsoft.AspNetCore.Identity;
using Plantilla.WebApi.Data.Contracts.Model;
using System;
using System.Collections.Generic;

namespace Plantilla.WebApi.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string HashedPassword { get; set; }

        public virtual ICollection<UserRoleDto> UserRoles { get; set; } = new List<UserRoleDto>();

        public string SetHashedPassWord(string password)
        {
            PasswordHasher<UserDto> hasher = new();
            HashedPassword = hasher.HashPassword(this, password);
            return HashedPassword;
        }

        public bool VerifyPassword(string password)
        {
            PasswordHasher<UserDto> hasher = new();
            var result = hasher.VerifyHashedPassword(this, HashedPassword, password);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
