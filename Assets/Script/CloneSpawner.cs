using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PrettyCloneSpawner : Singleton<PrettyCloneSpawner>
{
    [Header("References")]
    public Transform spawnOrigin;
    public Transform cloneParent;
    

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

    public List<Transform> clones = new List<Transform>();
    private BubbleNumberClone cloneGroup;

    public Color targetColor = Color.red;

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

    public void SpawnClones()
    {
        clones.Clear();

        if (cloneParent == null)
        {
            Debug.LogError("❌ cloneParent chưa được gán trong Inspector.");
            return;
        }

        if (spawnOrigin != null)
            cloneParent.position = spawnOrigin.position;

        cloneGroup = cloneParent.GetComponent<BubbleNumberClone>();
        if (cloneGroup == null)
            cloneGroup = cloneParent.gameObject.AddComponent<BubbleNumberClone>();

        for (int i = 0; i < numberOfClones; i++)
        {
            float radius = spacing * Mathf.Sqrt(i);
            float angle = i * angleFactor;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius + startZ;
            Vector3 targetLocalPos = new Vector3(x, 0f, z);

            GameObject clone = Singleton<ObjectPool>.Instance.GetObject();
            clone.transform.SetParent(cloneParent);
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localRotation = Quaternion.identity;
            clone.transform.localScale = Vector3.zero;

            clone.tag = "Clone_Player";
            clone.layer = LayerMask.NameToLayer("Clone_Player");

            if (clone.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            clone.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), scaleUpDuration).SetEase(Ease.OutBack);
            clone.transform.DOLocalMove(targetLocalPos, moveDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(i * 0.02f);

            clones.Add(clone.transform);
            ChangeColor(targetColor);
            ChangeAnimationState(CloneAnimState.Idle);
        }

        cloneGroup.UpdateCount();
    }

    bool IsOnGround(Transform clone)
    {
        Vector3 origin = clone.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, fallRayDistance, groundLayer);
    }

    // Khi clone bị rơi (vật lý)
    void DetachClone(Transform clone, bool isFalling = true)
    {
        clone.SetParent(null);

        if (!clone.TryGetComponent<Rigidbody>(out var rb))
            rb = clone.gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;

        if (isFalling)
        {
            // Hiệu ứng rơi rồi destroy
            Destroy(clone.gameObject, 0.5f);
        }
        else
        {
            // Trả về pool ngay (không hiệu ứng rơi)
            Singleton<ObjectPool>.Instance.ReturnObject(clone.gameObject);
        }
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Singleton<ObjectPool>.Instance.ReturnObject(obj);
    }

    public void ChangeColor(Color newColor)
    {
        targetColor = newColor;

        foreach (Transform clone in clones)
        {
            SkinnedMeshRenderer renderer = clone.GetComponentInChildren<SkinnedMeshRenderer>();
            if (renderer != null)
            {
                renderer.material.SetColor("_BaseColor", targetColor);
            }
            else
            {
                Debug.LogWarning($"Clone {clone.name} không có SkinnedMeshRenderer để đổi màu.");
            }
        }
    }

    public void ChangeAnimationState(CloneAnimState state)
    {
        foreach (Transform clone in clones)
        {
            if (clone == null) continue;

            CloneAnimatorController anim = clone.GetComponent<CloneAnimatorController>();
            if (anim == null) continue;

            switch (state)
            {
                case CloneAnimState.Idle:
                    anim.PlayIdle();
                    break;
                case CloneAnimState.Running:
                    anim.PlayRunning();
                    break;
            }
        }
    }

    // Hàm tạo một clone mới tại vị trí localPos (hoặc Vector3.zero nếu không truyền)
    private Transform CreateClone(Vector3? localPos = null, Color? color = null)
    {
        GameObject clone = Singleton<ObjectPool>.Instance.GetObject();
        clone.transform.SetParent(cloneParent);
        clone.transform.localPosition = localPos ?? Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        clone.tag = "Clone_Player";
        clone.layer = LayerMask.NameToLayer("Clone_Player");

        if (clone.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Đổi màu cho clone mới
        Color useColor = color ?? targetColor;
        SkinnedMeshRenderer renderer = clone.GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            renderer.material.SetColor("_BaseColor", useColor);
        }

        ChangeAnimationState(CloneAnimState.Idle);

        return clone.transform;
    }

    // Sắp xếp lại vị trí các clone theo đội hình tròn/quạt
    private void RearrangeClones()
    {
        for (int i = 0; i < clones.Count; i++)
        {
            float radius = spacing * Mathf.Sqrt(i);
            float angle = i * angleFactor;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius + startZ;
            Vector3 targetLocalPos = new Vector3(x, 0f, z);

            clones[i].localPosition = targetLocalPos;
        }
    }

    // Tăng số lượng clone lên amount
    public void AddClones(int amount)
    {
        Color baseColor = clones.Count > 0
            ? clones[0].GetComponentInChildren<SkinnedMeshRenderer>()?.material.GetColor("_BaseColor") ?? targetColor
            : targetColor;

        for (int i = 0; i < amount; i++)
        {
            clones.Add(CreateClone(null, baseColor));
        }
        RearrangeClones();
        cloneGroup?.UpdateCount();
    }

    // Giảm số lượng clone đi amount
    public void RemoveClones(int amount)
    {
        for (int i = 0; i < amount && clones.Count > 0; i++)
        {
            Transform clone = clones[clones.Count - 1];
            clones.RemoveAt(clones.Count - 1);
            DetachClone(clone);
        }
        RearrangeClones();
        cloneGroup?.UpdateCount();
    }

    // Nhân số lượng clone lên factor
    public void MultiplyClones(int factor)
    {
        if (factor <= 1 || clones.Count == 0) return;

        int originalCount = clones.Count;
        List<Vector3> originalPositions = new List<Vector3>();
        foreach (var clone in clones)
            originalPositions.Add(clone.localPosition);

        Color baseColor = clones[0].GetComponentInChildren<SkinnedMeshRenderer>()?.material.GetColor("_BaseColor") ?? targetColor;

        for (int i = 1; i < factor; i++)
        {
            for (int j = 0; j < originalCount; j++)
            {
                clones.Add(CreateClone(originalPositions[j], baseColor));
            }
        }
        RearrangeClones();
        cloneGroup?.UpdateCount();
    }

    // Chia số lượng clone cho divisor (làm tròn xuống)
    public void DivideClones(int divisor)
    {
        if (divisor <= 1) return;
        int targetCount = clones.Count / divisor;
        int removeCount = clones.Count - targetCount;
        RemoveClones(removeCount);
        RearrangeClones();
        cloneGroup?.UpdateCount();
    }

}
