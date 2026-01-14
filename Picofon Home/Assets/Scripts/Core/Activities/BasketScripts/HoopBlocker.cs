using UnityEngine;

public class HoopBlocker : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private AudioClip _stealClip;

    public void OnCollisionEnter2D(Collision2D _)
    {
        AudioManager.Instance.PlaySFX(_stealClip, 0.8f);
    }
}
