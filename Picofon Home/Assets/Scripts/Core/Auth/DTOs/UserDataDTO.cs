using Picofon.Utils;

namespace Picofon.Core.Auth.DTOs
{
    public class UserDataDTO
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool ProfileComplete { get; set; }

        public bool LegalAccepted { get; set; }

        public UserRole Role { get; set; }
    }
}
