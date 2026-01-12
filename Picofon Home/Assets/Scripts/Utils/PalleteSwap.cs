using UnityEngine;

public class PalleteSwapUtil : MonoBehaviour
{
    [SerializeField]
    private Color _targetColor = Color.red;

    [SerializeField]
    [Range(0f, 1f)]
    private float _weight = 1f;

    private MaterialPropertyBlock propertyBlock;

    public void OnValidate()
    {
        propertyBlock ??= new MaterialPropertyBlock();

        Renderer renderer = GetComponent<Renderer>();
        renderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_TargetColor", _targetColor);
        propertyBlock.SetFloat("_Weight", _weight);

        renderer.SetPropertyBlock(propertyBlock);
    }

    public void Start()
    {
        OnValidate();
    }
}
