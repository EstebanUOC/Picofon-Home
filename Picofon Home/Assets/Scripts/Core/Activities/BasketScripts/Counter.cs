using TMPro;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _counterText;

    private int _score;

    private readonly char[] scoreChars = new char[3];

    public void AddScore(bool correct)
    {
        if (!correct)
            return;

        _score++;
        UpdateScore(_score);
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
