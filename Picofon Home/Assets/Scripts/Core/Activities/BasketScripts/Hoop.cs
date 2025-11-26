using UnityEngine;

public class Hoop : MonoBehaviour
{
    [SerializeField]
    private Transform _ballTarget;

    public Transform BallTarget => _ballTarget;
}
