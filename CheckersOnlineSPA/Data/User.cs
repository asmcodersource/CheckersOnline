using System.ComponentModel.DataAnnotations;

namespace CheckersOnlineSPA.Data
{
    public class User
    {
        public User() { }
        public int Id { get; set; }
        [Required, MaxLength(24), MinLength(2)]
        public string UserName { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public string? Picture { get; set; } = null;
    }
}
