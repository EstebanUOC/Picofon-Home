using System;
using UnityEngine;

public class AnswerControllerJG : MonoBehaviour
{
    [Space(15)]
    public Answer PositiveAnswer;
    public Answer NegativeAnswer;

    public event Action<HoopType> OnAnswerSelected;

    public void Start()
    {
        PositiveAnswer.OnClick += () => AnswerSelected(HoopType.Positive);
        NegativeAnswer.OnClick += () => AnswerSelected(HoopType.Negative);
    }

    private void AnswerSelected(HoopType hoopType)
    {
        OnAnswerSelected?.Invoke(hoopType);
    }
}
