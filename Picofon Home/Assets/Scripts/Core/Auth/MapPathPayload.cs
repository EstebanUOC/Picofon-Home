using Picofon.Utils;

namespace Picofon.Core.Auth
{
    public enum NextAction : byte
    {
        Load = 0,
        Reload = 1,
        CalculateLR = 2,
    }

    public static class MapPathPayload
    {
        public static string ChildId;

        public static LanguageID LanguageId;

        public static string ConductedById;

        public static bool IsAIEnabled;

        public static NextAction NextAction;
    }
}
