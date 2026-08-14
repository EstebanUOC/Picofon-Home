using Picofon.Utils;

namespace Picofon.Core.Auth.Models.User
{
    public readonly struct UserModel
    {
        public readonly string Id { get; init; }

        public readonly string FirstName { get; init; }

        public readonly string Email { get; init; }

        public readonly bool LegalAccepted { get; init; }

        public readonly bool ProfileCompleted { get; init; }

        public readonly UserRole Role { get; init; }
    }
}
