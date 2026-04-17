using System;
using UnityEngine;

public class AnswerManagerJG : MonoBehaviour
{
    [Space(15)]
    public Answer PositiveAnswer;
    public Answer NegativeAnswer;

    public event Action<HoopType> OnAnswerSelected;

    public void Start()
    {
        PositiveAnswer.OnClick += () => AnswerSelected(HoopType.Si);
        NegativeAnswer.OnClick += () => AnswerSelected(HoopType.No);
    }

    private void AnswerSelected(HoopType hoopType)
    {
        OnAnswerSelected?.Invoke(hoopType);
    }

    public void DisableAnswers()
    {
        PositiveAnswer.Enabled = false;
        NegativeAnswer.Enabled = false;
    }

    public void EnableAnswers()
    {
        PositiveAnswer.Enabled = true;
        NegativeAnswer.Enabled = true;
    }
}
