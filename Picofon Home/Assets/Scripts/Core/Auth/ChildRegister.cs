using TMPro;
using UnityEngine;

public class ChildRegister : MonoBehaviour
{
    [Header("Parent Info")]
    [SerializeField]
    private TMP_Text parentEmail;

    [SerializeField]
    private TMP_Text parentUsername;

    public void SetParentInfo(string email, string username)
    {
        parentEmail.text = email;
        parentUsername.text = username;
    }
}
