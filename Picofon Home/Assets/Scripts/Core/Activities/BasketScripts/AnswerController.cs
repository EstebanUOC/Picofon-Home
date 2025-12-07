using UnityEngine;

public class AnswerController : MonoBehaviour
{
    [Space(15)]
    public Answer PositiveAnswer;
    public Answer NegativeAnswer;

    public void Start()
    {
        PositiveAnswer.OnClick += () => BasketManager.Instance.LaunchBall(HoopType.Positive);
        NegativeAnswer.OnClick += () => BasketManager.Instance.LaunchBall(HoopType.Negative);
    }
}
