using UnityEngine;

public enum CloneAnimState
{
    Idle,
    Running,
    Dancing
}

public class CloneAnimatorController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogWarning($"{name} ❌ không tìm thấy Animator.");
    }

    public void PlayRunning()
    {
        if (animator != null)
            animator.CrossFade("Running", 0.2f);
    }

    public void PlayIdle()
    {
        if (animator != null)
            animator.CrossFade("Idle", 0.2f);
    }
    public void PlayDancing()
    {
        if (animator != null)
            animator.CrossFade("Dance", 0.2f);
    }
}