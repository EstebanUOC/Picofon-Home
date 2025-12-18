using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    [Space(15)]
    public Image ImageLeft;
    public Image ImageRight;

    public void Start()
    {
        BasketManager.Instance.OnActivityChange += UpdateImages;
    }

    private void UpdateImages(BasketResponses.Activity activity)
    {
        Sprite leftSprite = LoadSprite(activity.Words[0].Path);
        Sprite rightSprite = LoadSprite(activity.Words[1].Path);

        ImageLeft.sprite = leftSprite;
        ImageRight.sprite = rightSprite;

        BasketManager.Instance.LeftSprite = leftSprite;
        BasketManager.Instance.RightSprite = rightSprite;
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
