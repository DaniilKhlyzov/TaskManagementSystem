using System;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; } = "User";
        
        public User()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}