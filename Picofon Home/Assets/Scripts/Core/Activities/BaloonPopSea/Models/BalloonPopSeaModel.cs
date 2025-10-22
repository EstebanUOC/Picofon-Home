using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class  ActivityOption
{
 public string text;
     public string path;
  public int id;

}
[System.Serializable]
public class Activity
{
    public string question;
    public string main_word;
    public string main_word_PATH;
    public int main_word_id;
    public ActivityOption correct_option;
    public ActivityOption wrong_option1;
    public ActivityOption wrong_option2;
    public bool answer;
    public string feedback_positive;
    public string feedback_neutral;
    public string feedback_no_answer;
    public List<int> used_word_ids;
}

[System.Serializable]
public class AvailabilityInfo
{
    public int words_available;
    public int questions_possible;
    public int therapy_plan_requested;
    public int words_per_question;
    public bool sufficient_words;
    public int activity_number;
    public int activities_created;
    public int activities_requested;
}

[System.Serializable]
public class Data
{
    public Activity activity1;
    public Activity activity2;
    public Activity activity3;
    public AvailabilityInfo availability_info;
}


[System.Serializable]
public class ApiResponse
{
    public bool success;
    public string message;
    public Data data;
}