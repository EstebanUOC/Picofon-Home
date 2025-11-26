using UnityEngine;

public static class GamePrefs
{
    public static int LastCompletedLevel
    {
        get => PlayerPrefs.GetInt(nameof(LastCompletedLevel), 0);
        set
        {
            PlayerPrefs.SetInt(nameof(LastCompletedLevel), value);
            Save();
        }
    }

    public static bool HasAcceptedTerms
    {
        get => PlayerPrefs.GetInt(nameof(HasAcceptedTerms), 0) == 1;
        set
        {
            PlayerPrefs.SetInt(nameof(HasAcceptedTerms), value ? 1 : 0);
            Save();
        }
    }

    private static void Save()
    {
        PlayerPrefs.Save();
    }
}
