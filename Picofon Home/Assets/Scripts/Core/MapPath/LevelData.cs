using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public Sprite LevelIcon;
    public string SceneName;
}
