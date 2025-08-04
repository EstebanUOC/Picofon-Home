using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WordSegment
{
    public int id;
    public string syllabified_word;

    public string GetPrimeraSilaba()
    {
        return syllabified_word.Split('#')[0];
    }

    public string GetPalabra()
    {
        return syllabified_word.Replace("#", "");
    }
}

