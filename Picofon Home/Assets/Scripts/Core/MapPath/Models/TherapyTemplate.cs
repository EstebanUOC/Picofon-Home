namespace Picofon.Core.MapPath.Models
{
    public class TherapyTemplate
    {
        public int SkillId { get; init; }

        public int Id { get; init; }

        public bool IsActive { get; init; }

        public int TaskTypeId { get; init; }

        // SPS (Segmentar y contar sílabas) tiene escenas e icono propios
        public const int SpsSkillId = 4;

        public bool IsSps => SkillId == SpsSkillId;
    }
}
