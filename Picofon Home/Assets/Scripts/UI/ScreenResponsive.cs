namespace Picofon.UI
{
    using Picofon.Utils;
    using UnityEngine;

    public class ScreenResponsive : MonoBehaviour
    {
        RectTransform panel;
        Rect lastSafeArea = new(0, 0, 0, 0);

        void Awake()
        {
            Application.targetFrameRate = 60;
            panel = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;
            PerformanceLog.Log("Safe Area: " + safeArea);
            lastSafeArea = safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
        }
    }
}
