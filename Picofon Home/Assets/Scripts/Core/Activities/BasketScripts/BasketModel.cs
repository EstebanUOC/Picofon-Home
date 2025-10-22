using System;
using UnityEngine;

[Serializable]
public class BasketWord
{
    public string word;
    public string PATH;
    public int id;
    public string syllabified_word;
}

[Serializable]
public class BasketActivity
{
    public string question;
    public BasketWord word1;
    public BasketWord word2;
    public bool answer; // true = same syllable, false = different
    public string answer_type;
    public string feedback_positive;
    public string feedback_neutral;
    public string feedback_no_answer;
}

[Serializable]
public class BasketData
{
    public BasketActivity activity1;
    public BasketActivity activity2;
    public BasketActivity activity3;
}

[Serializable]
public class BasketResponse
{
    public bool success;
    public BasketData data;
}
