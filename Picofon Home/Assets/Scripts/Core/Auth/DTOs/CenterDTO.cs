namespace Picofon.Core.Auth.DTOs
{
    public readonly struct CenterDTO
    {
        public readonly int Id { get; init; }

        public readonly string Name { get; init; }

        public readonly int CountryId { get; init; }
    }
}
