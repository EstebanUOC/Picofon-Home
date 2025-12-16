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
        ImageLeft.sprite = LoadSprite(activity.Words[0].Path);
        ImageRight.sprite = LoadSprite(activity.Words[1].Path);
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
