using Microsoft.AspNetCore.Identity;

namespace PSA_Business_Logic.Model
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Discriminator { get; set; }
        public string? Address { get; set; }
        public string? District { get; set; }
    }
}
