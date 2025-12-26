using UnityEngine;

public class ColorSwapUtil : MonoBehaviour
{
    public Color color;

    private MaterialPropertyBlock propertyBlock;

    public void OnValidate()
    {
        propertyBlock ??= new MaterialPropertyBlock();

        Renderer renderer = GetComponent<Renderer>();
        renderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetColor("_TargetColor", color);

        renderer.SetPropertyBlock(propertyBlock);
    }
}
