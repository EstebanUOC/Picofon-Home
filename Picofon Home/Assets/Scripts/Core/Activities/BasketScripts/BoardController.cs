using UnityEngine;

public class BoardController : MonoBehaviour
{
    [Space(15)]
    public GameFrameController FrameLeft;
    public GameFrameController FrameRight;

    public void Start()
    {
        BasketManager.Instance.OnActivityChange += UpdateImages;
        BasketManager.Instance.OnClueActived += ShowClues;
    }

    private void UpdateImages(in BasketResponses.BasketActivity activity)
    {
        Sprite leftSprite = activity.LeftImage;
        Sprite rightSprite = activity.RightImage;

        FrameLeft.UpdateFrame(leftSprite, activity.LeftWord);
        FrameRight.UpdateFrame(rightSprite, activity.RightWord);
    }

    private void ShowClues(bool active)
    {
        if (active)
        {
            FrameLeft.ShowClue();
            FrameRight.ShowClue();
        }
        else
        {
            FrameLeft.HideClue();
            FrameRight.HideClue();
        }
    }
}
