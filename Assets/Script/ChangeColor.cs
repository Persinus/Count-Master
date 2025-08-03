using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    void Start()
    {
        // Tìm SkinnedMeshRenderer trong con của GameObject này
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            // Đổi màu của material
            renderer.material.SetColor("_BaseColor", Color.green); // với URP
        }
        else
        {
            Debug.LogWarning("Không tìm thấy SkinnedMeshRenderer.");
        }
    }
}
