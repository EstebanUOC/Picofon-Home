namespace Picofon.Activities.Basket
{
    using Cysharp.Threading.Tasks;
    using PrimeTween;
    using UnityEngine;
    using UnityEngine.UI;

    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private GameObject _starPrefab;

        [SerializeField]
        private Transform _starContainer;

        [SerializeField]
        private RectTransform _rocket;

        [SerializeField]
        private RectTransform _fill;

        private readonly Color32 _positiveColor = new(130, 208, 210, 255);
        private readonly Color32 _negativeColor = new(210, 130, 133, 255);

        private bool _completed;

        public void Initialize(int parts, bool completed)
        {
            if (parts <= 0)
                return;

            int starCount = _starContainer.childCount;

            if (starCount < parts)
            {
                for (int i = starCount; i < parts; i++)
                {
                    Instantiate(_starPrefab, _starContainer);
                }
            }

            if (starCount > parts)
            {
                for (int i = parts; i < starCount; i++)
                {
                    _starContainer.GetChild(i).gameObject.SetActive(false);
                }
            }

            _completed = completed;

            if (completed)
            {
                FillProgressBar(parts).Forget();
            }
        }

        public void SetProgress(int progress, bool correct)
        {
            if (_completed)
                return;

            int index = progress - 1;

            RectTransform star = _starContainer.GetChild(index) as RectTransform;
            Image image = star.GetComponent<Image>();

            Vector2 size = _fill.sizeDelta;
            Vector2 position = Vector2.right * star.anchoredPosition.x;

            size.x = star.anchoredPosition.x;

            Color32 targetColor = correct ? _positiveColor : _negativeColor;

            Sequence
                .Create()
                .Group(Tween.UIAnchoredPosition(_rocket, endValue: position, duration: 0.5f))
                .Group(Tween.UISizeDelta(_fill, endValue: size, duration: 0.5f))
                .Chain(
                    Tween.Color(image, endValue: targetColor, duration: 0.3f, ease: Ease.OutBack)
                );
        }

        private async UniTaskVoid FillProgressBar(int parts)
        {
            await UniTask.WaitForEndOfFrame(this);

            RectTransform star = null;

            for (int i = 0; i < parts; i++)
            {
                star = _starContainer.GetChild(i) as RectTransform;
                Image image = star.GetComponent<Image>();
                image.color = _positiveColor;
            }

            Vector2 size = _fill.sizeDelta;
            Vector2 position = Vector2.right * star.anchoredPosition.x;

            size.x = star.anchoredPosition.x;

            _rocket.anchoredPosition = position;
            _fill.sizeDelta = size;
        }
    }
}
