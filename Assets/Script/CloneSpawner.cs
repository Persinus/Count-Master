using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;



public class PrettyCloneSpawner : Singleton<PrettyCloneSpawner>
{
    [Header("References")]
    [SerializeField] private Transform spawnOrigin;
    [SerializeField] private Transform cloneParent;

    [Header("Spawn Settings")]
    public int numberOfClones = 30;
    public float spacing = 0.4f;
    public float startZ = -15f;
    public float angleFactor = 0.5f;
    public Vector3 cloneScale = new Vector3(0.7f, 0.7f, 0.7f);
    public float spawnDelayStep = 0.02f;

    [Header("Effect Settings")]
    public float scaleUpDuration = 0.4f;
    public float moveDuration = 0.5f;

    [Header("Fall Settings")]
    public float fallRayDistance = 0.4f;
    public float yFallThreshold = -10f;
    public LayerMask groundLayer;

    [Header("Visuals")]
    public Color targetColor = Color.red;

    public List<CloneData> clones = new List<CloneData>();
    private BubbleNumberClone cloneGroup;

    private void Awake()
    {
        if (cloneParent != null)
            cloneGroup = cloneParent.GetComponent<BubbleNumberClone>() ?? cloneParent.gameObject.AddComponent<BubbleNumberClone>();
    }

    private void Update()
    {
        for (int i = clones.Count - 1; i >= 0; i--)
        {
            var clone = clones[i];
            if (clone == null || clone.transform == null) continue;

            bool isFalling = !IsOnGround(clone.transform) || clone.transform.position.y < yFallThreshold;
            if (isFalling)
            {
                DetachClone(clone.transform);
                clones.RemoveAt(i);
                cloneGroup?.UpdateCount();
            }
        }
    }

    public void SpawnClones()
    {
        ClearClones();

        if (cloneParent == null)
        {
            Debug.LogError("❌ cloneParent chưa được gán trong Inspector.");
            return;
        }

        if (spawnOrigin != null)
            cloneParent.position = spawnOrigin.position;

        for (int i = 0; i < numberOfClones; i++)
        {
            float radius = spacing * Mathf.Sqrt(i);
            float angle = i * angleFactor;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius + startZ;
            Vector3 targetLocalPos = new Vector3(x, 0f, z);

            var cloneData = InitClone(cloneParent, Vector3.zero, targetColor);
            clones.Add(cloneData);

            cloneData.transform.localScale = Vector3.zero;
            cloneData.transform.DOScale(cloneScale, scaleUpDuration).SetEase(Ease.OutBack);
            cloneData.transform.DOLocalMove(targetLocalPos, moveDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(i * spawnDelayStep);
        }

        cloneGroup?.UpdateCount();
    }

    private CloneData InitClone(Transform parent, Vector3 localPos, Color color)
    {
        GameObject clone = Singleton<ObjectPool>.Instance.GetObject();
        clone.transform.SetParent(parent);
        clone.transform.localPosition = localPos;
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.localScale = cloneScale;

        clone.tag = "Clone_Player";
        clone.layer = LayerMask.NameToLayer("Clone_Player");

        if (clone.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        var renderer = clone.GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer != null)
            renderer.material.SetColor("_BaseColor", color);

        var animator = clone.GetComponent<CloneAnimatorController>();
        if (animator != null)
            animator.PlayIdle();

        return new CloneData
        {
            transform = clone.transform,
            renderer = renderer,
            animator = animator
        };
       
    }

    private bool IsOnGround(Transform clone)
    {
        Vector3 origin = clone.position + Vector3.up * 0.2f;
        return Physics.Raycast(origin, Vector3.down, fallRayDistance, groundLayer);
    }

    private void DetachClone(Transform clone, bool isFalling = true)
    {
        clone.SetParent(null);

        if (!clone.TryGetComponent<Rigidbody>(out var rb))
            rb = clone.gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;

        if (isFalling)
            StartCoroutine(ReturnToPoolAfterDelay(clone.gameObject, 0.5f));
        else
            Singleton<ObjectPool>.Instance.ReturnObject(clone.gameObject);
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Singleton<ObjectPool>.Instance.ReturnObject(obj);
    }

    public void ChangeColor(Color newColor)
    {
        targetColor = newColor;
        foreach (var clone in clones)
            if (clone.renderer != null)
                clone.renderer.material.SetColor("_BaseColor", targetColor);
    }

    public void ChangeAnimationState(CloneAnimState state)
{
    if (clones.Count == 0) return;

    float normalizedTime = 0f;

    // Lấy normalizedTime từ clone đầu tiên nếu có animator
    var firstAnimData = clones[0].animator;
    if (firstAnimData != null)
    {
        Animator animator = firstAnimData.GetComponent<Animator>();
        if (animator != null)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            normalizedTime = stateInfo.normalizedTime % 1f;
        }
    }

    foreach (var clone in clones)
    {
        if (clone.animator == null) continue;

        switch (state)
        {
            case CloneAnimState.Idle:
                clone.animator.PlayIdle();
                break;
            case CloneAnimState.Running:
                clone.animator.PlayRunning(normalizedTime);
                break;
            case CloneAnimState.Dancing:
                clone.animator.PlayDancing();
                break;
        }
    }
}


    public void AddClones(int amount)
    {
        Color baseColor = clones.Count > 0 && clones[0].renderer != null
            ? clones[0].renderer.material.GetColor("_BaseColor")
            : targetColor;

        for (int i = 0; i < amount; i++)
            clones.Add(InitClone(cloneParent, Vector3.zero, baseColor));

        RearrangeClones();
        cloneGroup?.UpdateCount();
    }

    public void RemoveClones(int amount)
    {
        for (int i = 0; i < amount && clones.Count > 0; i++)
        {
            var clone = clones[^1];
            clones.RemoveAt(clones.Count - 1);
            DetachClone(clone.transform);
        }
        RearrangeClones();
        cloneGroup?.UpdateCount();
    }

    public void MultiplyClones(int factor)
    {
        if (factor <= 1 || clones.Count == 0) return;

        int originalCount = clones.Count;
        List<Vector3> originalPositions = new List<Vector3>();
        foreach (var c in clones)
            originalPositions.Add(c.transform.localPosition);

        Color baseColor = clones[0].renderer != null
            ? clones[0].renderer.material.GetColor("_BaseColor")
            : targetColor;

        for (int i = 1; i < factor; i++)
            for (int j = 0; j < originalCount; j++)
                clones.Add(InitClone(cloneParent, originalPositions[j], baseColor));

        RearrangeClones();
        cloneGroup?.UpdateCount();
    }

    public void DivideClones(int divisor)
    {
        if (divisor <= 1) return;
        int targetCount = clones.Count / divisor;
        int removeCount = clones.Count - targetCount;
        RemoveClones(removeCount);
        RearrangeClones();
        cloneGroup?.UpdateCount();
    }

    private void RearrangeClones()
    {
        for (int i = 0; i < clones.Count; i++)
        {
            float radius = spacing * Mathf.Sqrt(i);
            float angle = i * angleFactor;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius + startZ;
            Vector3 targetLocalPos = new Vector3(x, 0f, z);

            clones[i].transform.DOLocalMove(targetLocalPos, 0.3f).SetEase(Ease.OutQuad);
        }
    }

    private void ClearClones()
    {
        foreach (var clone in clones)
            if (clone != null && clone.transform != null)
                Singleton<ObjectPool>.Instance.ReturnObject(clone.transform.gameObject);
        clones.Clear();
    }
     



    [System.Serializable]
public class CloneData
{
    public Transform transform;
    public SkinnedMeshRenderer renderer;
    public CloneAnimatorController animator;
}
}
