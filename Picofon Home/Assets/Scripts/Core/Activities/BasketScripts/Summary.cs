using System;
using UnityEngine;
using UnityEngine.UI;

public class Summary : MonoBehaviour
{
    public event Action OnSummaryCompleted;

    [SerializeField]
    private Button _continueButton;

    public void Start()
    {
        _continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void OnContinueClicked()
    {
        OnSummaryCompleted?.Invoke();
    }
}
