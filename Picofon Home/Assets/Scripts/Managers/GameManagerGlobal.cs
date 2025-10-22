using UnityEngine;

public class GameManagerGlobal : MonoBehaviour
{
    [SerializeField] private ActivityBaseManager activityManager;
    [SerializeField, Range(0, 3)] private int initialMode = 0; // 0=Judge,1=Relate,2=Create,3=Select

    private void Start()
    {
        if (activityManager != null)
            activityManager.LoadMode(initialMode);
        else
            Debug.LogError("❌ No se asignó ActivityManager en el GameManagerGlobal.");
    }
}
