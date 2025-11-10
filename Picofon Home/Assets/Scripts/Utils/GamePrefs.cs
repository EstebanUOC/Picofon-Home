using UnityEngine;

public static class GamePrefs
{
    public static int LastCompletedLevel
    {
        get { return PlayerPrefs.GetInt(nameof(LastCompletedLevel), 0); }
        set { PlayerPrefs.SetInt(nameof(LastCompletedLevel), value); }
    }
}
