using UnityEngine;

public class ColorSwapUtil : MonoBehaviour
{
    public Color originalColor = Color.white;
    public Color targetColor = Color.red;

    private MaterialPropertyBlock propertyBlock;

    public void OnValidate()
    {
        propertyBlock ??= new MaterialPropertyBlock();

        Renderer renderer = GetComponent<Renderer>();
        renderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_OriginalColor", originalColor);
        propertyBlock.SetColor("_TargetColor", targetColor);

        renderer.SetPropertyBlock(propertyBlock);
    }

    public void Start()
    {
        OnValidate();
    }
}
