using TMPro;
using UnityEngine;

public class ChildRegister : MonoBehaviour
{
    [Header("Parent Info")]
    [SerializeField]
    private TMP_Text EmailText;

    [SerializeField]
    private TMP_Text UsernameText;

    public void SetParentInfo(string email, string username)
    {
        EmailText.text = email;
        UsernameText.text = username;
    }
}
