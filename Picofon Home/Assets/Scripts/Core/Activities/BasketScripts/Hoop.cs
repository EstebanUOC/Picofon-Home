using UnityEngine;

public class Hoop : MonoBehaviour
{
    [Space(15)]
    public Transform TargetPosition;

    public bool Blocked
    {
        get { return _blocked; }
        set { _blocked = value; }
    }

    public HoopCollider Collider
    {
        set
        {
            _collider = value;
            _collider.Blocked = _blocked;
            _collider.transform.position = transform.position;
            _collider.gameObject.SetActive(true);
        }
    }

    private HoopCollider _collider;
    private bool _blocked = false;
}
