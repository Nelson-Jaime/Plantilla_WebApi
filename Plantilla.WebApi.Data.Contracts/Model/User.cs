using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Plantilla.WebApi.Data.Contracts.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string HashedPassword { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public string SetHashedPassWord(string password)
        {
            PasswordHasher<User> hasher = new();
            HashedPassword = hasher.HashPassword(this, password);
            return HashedPassword;
        }

        public bool VerifyPassword(string password)
        {
            PasswordHasher<User> hasher = new();
            var result = hasher.VerifyHashedPassword(this, HashedPassword, password);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
