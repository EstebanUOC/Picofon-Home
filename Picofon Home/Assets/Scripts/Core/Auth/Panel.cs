using System.Collections;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public float FadeDuration = 10;

    private CanvasGroup canvasGroup;

    public void Awake()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        StartCoroutine(FadeIn());
    }

    public virtual void Hide()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(FadeOut());
        }
    }

    public IEnumerator FadeIn()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        yield return FadeAlpha(true);

        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        Debug.Log("Hiding panel: " + gameObject.name);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        yield return FadeAlpha(false);

        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    private IEnumerator FadeAlpha(bool fadeIn)
    {
        for (float t = 0; t < FadeDuration; t += Time.deltaTime)
        {
            float val = fadeIn ? (t / FadeDuration) : (1 - (t / FadeDuration));
            canvasGroup.alpha = val;

            yield return null;
        }
    }
}
