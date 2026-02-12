using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Summary : MonoBehaviour
{
    [SerializeField]
    private Button _continueButton;

    public void Start()
    {
        _continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void OnContinueClicked()
    {
        SceneManager.LoadScene("MapPathScene");
    }
}
