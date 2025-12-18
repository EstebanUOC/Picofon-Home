using UnityEngine;

public class BoardController : MonoBehaviour
{
    [Space(15)]
    public ImageFrameController FrameLeft;
    public ImageFrameController FrameRight;

    public void Start()
    {
        BasketManager.Instance.OnActivityChange += UpdateImages;
        BasketManager.Instance.OnClueActived += ShowClues;
    }

    private void UpdateImages(BasketResponses.Activity activity)
    {
        Sprite leftSprite = LoadSprite(activity.Words[0].Path);
        Sprite rightSprite = LoadSprite(activity.Words[1].Path);

        BasketManager.Instance.LeftSprite = leftSprite;
        BasketManager.Instance.RightSprite = rightSprite;

        FrameLeft.UpdateFrame(leftSprite, activity.Words[0].Word);
        FrameRight.UpdateFrame(rightSprite, activity.Words[1].Word);
    }

    private void ShowClues(bool active)
    {
        if (active)
        {
            FrameLeft.ShowText();
            FrameRight.ShowText();
        }
    }

    private Sprite LoadSprite(string p)
    {
        string file = System.IO.Path.GetFileNameWithoutExtension(p);
        Sprite s = Resources.Load<Sprite>($"Images/ImgButtons/{file}");

        if (!s)
            Debug.LogWarning($"No se encontró sprite: {file}");

        return s;
    }
}
