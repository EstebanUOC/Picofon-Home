using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;

namespace Picofon.Components
{
    public class Counter : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _counterText;

        [SerializeField]
        private bool _prueba = false;

        private int _score;

        private readonly char[] scoreChars = new char[3];

        public void Start()
        {
            if (_prueba)
            {
                RectTransform rectTransform = transform as RectTransform;

                Vector3 position = rectTransform.anchoredPosition;
                position.x = 700;

                rectTransform.anchoredPosition = position;
            }
        }

        public async UniTaskVoid Show()
        {
            RectTransform rectTransform = transform as RectTransform;

            _ = Tween.UIAnchoredPositionX(
                rectTransform,
                endValue: -50f,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            await UniTask.WaitForSeconds(3f);

            _ = Tween.UIAnchoredPositionX(
                rectTransform,
                endValue: 700f,
                duration: 0.5f,
                ease: Ease.InCubic
            );
        }

        public void AddScore(bool correct)
        {
            if (!correct)
                return;

            _score++;
            UpdateScore(_score);
        }

        public void SetScore(int score)
        {
            if (score < 0)
                return;

            UpdateScore(score);
        }

        private void UpdateScore(int score)
        {
            int length = ConvertIntToCharArray(score, scoreChars, 0);
            _counterText.SetCharArray(scoreChars, 0, length);

            _score = score;
        }

        private int ConvertIntToCharArray(int value, char[] buffer, int startIndex)
        {
            if (value == 0)
            {
                buffer[startIndex] = '0';
                return 1;
            }

            int tempValue = value;

            int digitCount = 0;
            while (tempValue > 0)
            {
                tempValue /= 10;
                digitCount++;
            }

            for (int i = digitCount - 1; i >= 0; i--)
            {
                buffer[startIndex + i] = (char)('0' + (value % 10));
                value /= 10;
            }

            return digitCount;
        }
    }
}
