namespace Picofon.Core.MapPath
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class PruebaScroll : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        public ScrollRect scrollRect;
        public RectTransform content;

        private float maxStretch = 0.35f;
        private float stretchMultiplier = 0.5f;

        private bool _scaledUp = false;
        private float _difference;

        public void Start()
        {
            // _tween = content.DOScale(Vector3.one, 0.1f).SetAutoKill(false).Pause();

            float size = GetComponent<RectTransform>().rect.height;
            float contentSize = content.rect.height;
            _difference = contentSize - size;
        }

        public void OnDrag(PointerEventData eventData)
        {
            float ny = scrollRect.verticalNormalizedPosition;

            if (ny <= 1 && ny >= 0 || ny - 1 < 0.001f)
            {
                return;
            }

            Debug.Log("Ny: " + ny);
            Debug.Log("Scaling up" + (ny - 1));
            _scaledUp = true;
            float stretchY;

            float overBottom = Mathf.Max(0f, 0f - ny);
            float overTop = Mathf.Max(0f, ny - 1f);
            float overV = overTop + overBottom;
            stretchY = Mathf.Clamp(overV * stretchMultiplier, 0f, maxStretch);

            if (ny > 1)
            {
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0f);
            }

            if (ny < 0)
            {
                content.anchoredPosition = new Vector2(
                    content.anchoredPosition.x,
                    _difference + stretchY * content.rect.height
                );
            }

            Vector3 targetScale = Vector3.one;
            targetScale.y = 1f + stretchY;

            content.localScale = targetScale;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_scaledUp)
            {
                Debug.Log("Restarting tween");
                _scaledUp = false;
                // _tween.Restart();
            }
        }
    }
}
