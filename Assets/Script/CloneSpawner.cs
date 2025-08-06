using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PrettyCloneSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform spawnOrigin;        // Vị trí dùng làm gốc tính toán (có thể null nếu không cần)
    public Transform cloneParent;        // Transform chứa các clone (gán trong scene)

    [Header("Spawn Settings")]
    public int numberOfClones = 30;
    public float spacing = 0.4f;
    public float startZ = -15f;
    public float angleFactor = 0.5f;

    [Header("Effect Settings")]
    public float scaleUpDuration = 0.4f;
    public float moveDuration = 0.5f;

    [Header("Fall Settings")]
    public float fallRayDistance = 0.3f;
    public float yFallThreshold = -10f;
    public LayerMask groundLayer;

    private List<Transform> clones = new List<Transform>();
    private CloneGroup cloneGroup;

    Color targetColor = Color.red; // 🎯 Màu muốn đổi sang (bạn có thể sửa thành màu khác)


    void Start()
    {
        SpawnClones();
    }

    void Update()
    {
        for (int i = clones.Count - 1; i >= 0; i--)
        {
            Transform clone = clones[i];
            if (clone == null) continue;

            bool isFalling = !IsOnGround(clone) || clone.position.y < yFallThreshold;
            if (isFalling)
            {
                DetachClone(clone);
                clones.RemoveAt(i);
                cloneGroup?.UpdateCount();
            }
        }
    }

    void SpawnClones()
    {
        clones.Clear();

        if (cloneParent == null)
        {
            Debug.LogError("❌ cloneParent chưa được gán trong Inspector.");
            return;
        }

        // Set vị trí ban đầu (nếu cần thiết)
        if (spawnOrigin != null)
            cloneParent.position = spawnOrigin.position;

        // Lấy hoặc thêm CloneGroup
        cloneGroup = cloneParent.GetComponent<CloneGroup>();
        if (cloneGroup == null)
            cloneGroup = cloneParent.gameObject.AddComponent<CloneGroup>();

        for (int i = 0; i < numberOfClones; i++)
        {
            float radius = spacing * Mathf.Sqrt(i);
            float angle = i * angleFactor;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius + startZ;
            Vector3 targetLocalPos = new Vector3(x, 0f, z); // Vị trí cục bộ trong cha

            GameObject clone = ObjectPooler.Instance.GetFromPool();
            clone.transform.SetParent(cloneParent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localRotation = Quaternion.identity;
            clone.transform.localScale = Vector3.zero;

            clone.tag = "Clone_Player";
            clone.layer = LayerMask.NameToLayer("Clone_Player");

            // Tìm SkinnedMeshRenderer trong clone
            SkinnedMeshRenderer renderer = clone.GetComponentInChildren<SkinnedMeshRenderer>();
            if (renderer != null)
            {
            // Tạo material riêng để không ảnh hưởng các clone khác
           renderer.material = new Material(renderer.material);

            // Đổi màu clone – dùng "_BaseColor" cho URP, "_Color" cho Built-in
            renderer.material.SetColor("_BaseColor", Color.red); // dùng URP
            // renderer.material.SetColor("_Color", Color.green);   // nếu dùng Built-in
}
else
{
    Debug.LogWarning($"Không tìm thấy SkinnedMeshRenderer trong {clone.name}");
}

            if (clone.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            clone.transform.DOScale(Vector3.one, scaleUpDuration).SetEase(Ease.OutBack);
            clone.transform.DOLocalMove(targetLocalPos, moveDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(i * 0.02f);

            clones.Add(clone.transform);
        }

      
        cloneGroup.UpdateCount();
    }

    bool IsOnGround(Transform clone)
    {
        Vector3 origin = clone.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, fallRayDistance, groundLayer);
    }

    void DetachClone(Transform clone)
    {
        clone.SetParent(null);

        if (!clone.TryGetComponent<Rigidbody>(out var rb))
            rb = clone.gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;
        Destroy(clone.gameObject, 0.5f);

    }
}
