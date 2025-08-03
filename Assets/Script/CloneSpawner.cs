using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class PrettyCloneSpawner : MonoBehaviour
{
    public ObjectPooler pooler;
    public Transform parentTransform;

    [Header("Spawn Settings")]
    public int numberOfClones = 30;
    public float spacing = 0.4f;
    public float startZ = -15f;
    public float angleFactor = 0.5f;

    [Header("Effect Settings")]
    public float scaleUpDuration = 0.4f;
    public float moveDuration = 0.5f;
    public float riseHeight = 1.2f;
    public Gradient spawnColorGradient; // Thay đổi màu theo vị trí spawn

    private List<Transform> clones = new List<Transform>();

    void Start()
    {
        SpawnInSpiralFormation();
        SetupCenterCollider();
    }

    void SpawnInSpiralFormation()
{
    clones.Clear();

    for (int i = 0; i < numberOfClones; i++)
    {
        float radius = spacing * Mathf.Sqrt(i);
        float angle = i * angleFactor;

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius + startZ;

        Vector3 localTargetPos = new Vector3(x, 0f, z);

        // Lấy clone từ pool
        GameObject newClone = pooler.GetFromPool();

        // Ghi lại transform gốc từ prefab
        Vector3 originalScale = newClone.transform.localScale;
        Quaternion originalRotation = newClone.transform.localRotation;

        // Gắn vào parent
        newClone.transform.SetParent(parentTransform);

        // Reset vị trí về trung tâm spawn (zero), scale về 0, giữ rotation gốc
        newClone.transform.localPosition = Vector3.zero;
        newClone.transform.localRotation = originalRotation;
        newClone.transform.localScale = Vector3.zero;

        // Animate scale về kích thước ban đầu
        newClone.transform.DOScale(originalScale, scaleUpDuration)
            .SetEase(Ease.OutBack);

        // Animate nhảy đến vị trí đích
        newClone.transform.DOLocalMove(
    localTargetPos,
    moveDuration)
    .SetEase(Ease.OutBack)
    .SetDelay(i * 0.02f);

        // Vô hiệu hóa collider, rigidbody ban đầu
        if (newClone.TryGetComponent<Collider>(out var col)) col.enabled = false;
        if (newClone.TryGetComponent<Rigidbody>(out var rb)) Destroy(rb);

        clones.Add(newClone.transform);
    }
}


    void SetupCenterCollider()
    {
        if (clones.Count == 0) return;

        int centerIndex = clones.Count / 2;
        Transform centerClone = clones[centerIndex];

        // Bật collider
        Collider col = centerClone.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        // Thêm Rigidbody nếu cần
        Rigidbody rb = centerClone.GetComponent<Rigidbody>();
        if (rb == null) rb = centerClone.gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;
    }
}  