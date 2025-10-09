using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class JudgeController : MonoBehaviour
{
    [Header("🎯 References")]
    public Transform hoopLeft;
    public Transform hoopCenter;
    public TMP_Text hoopLeftLabel;
    public TMP_Text hoopCenterLabel;  
    public List<BallController> activeBalls = new List<BallController>();

    [Header("✅ Answer Settings")]
    public bool imagesHaveSameSyllable = true; // static for now

    public void ActivateTypeJudge()
    {
        hoopLeft.gameObject.SetActive(true);
        hoopCenter.gameObject.SetActive(true);

        hoopLeftLabel.text = "SI";
        hoopCenterLabel.text = " NO";

        hoopLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2(-350f, 0f);
        hoopCenter.GetComponent<RectTransform>().anchoredPosition = new Vector2(365f, 0f);
    }

    // Called when player clicks anywhere or a test key
    public void OnClickLeftHoop()
    {
        MoveBallsToTarget(hoopLeft);
    }

    public void OnClickRightHoop()
    {
        MoveBallsToTarget(hoopCenter);
    }

    void MoveBallsToTarget(Transform target)
    {
        Debug.Log($"🟠 Moving all balls toward: {target.name}");

        foreach (BallController ball in activeBalls)
        {
            if (ball != null)
            {
                ball.StartMoveTo(target);
            }
        }
    }
}
