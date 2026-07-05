public class CreateChildDTO
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string BirthDate { get; set; }

    public string Disorder { get; set; }

    public string School { get; set; }

    public int LanguagePreference { get; set; }

    public int Grade { get; set; }

    public int? CenterId { get; set; } = null;

    public string UserId { get; set; }

    public UserRole Relationship { get; set; }

    public bool IsAiPersonalizationEnabled { get; set; } = true;
}
