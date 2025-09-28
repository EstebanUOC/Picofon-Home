using UnityEngine;

[System.Serializable]
public class WordData
{
    public string main_word;
    public string wrong_option1;
    public string wrong_option2;
    public string wrong_option3;
    public string correct_option;
}


public static class WordLoader
{
    public static WordData LoadFromTextAsset(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName);
        if (jsonFile == null)
        {
            Debug.LogError($"No se encontró el archivo JSON: {fileName}");
            return null;
        }

        return JsonUtility.FromJson<WordData>(jsonFile.text);
    }
}
