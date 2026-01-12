using UnityEngine;

public class Hoop : MonoBehaviour
{
    [Space(15)]
    public Transform TargetPosition;

    [Space(15)]
    public CapsuleCollider2D ColliderLeft;
    public CapsuleCollider2D ColliderRight;
    public CapsuleCollider2D ColliderBlocker;

    [SerializeField]
    private CapsuleCollider2D _colliderSwish;

    public bool Blocked
    {
        get { return _blocked; }
        set
        {
            if (_blocked == value)
                return;

            _blocked = value;
            ColliderBlocker.enabled = value;
            ColliderLeft.enabled = !value;
            ColliderRight.enabled = !value;
        }
    }

    private bool _blocked = false;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Hoop triggered by {collision.name} - In: {name}");
    }
}
