using Microsoft.AspNetCore.Identity;

namespace Learning.Api
{
    public class ApplicationUser : IdentityUser
    {
        public string? NativeLanguage { get; set; }
        public string? LearningLanguage { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
