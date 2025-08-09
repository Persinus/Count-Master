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

    public void PlayRunning(float normalizedTime = 0f)
{
    if (animator != null)
    {
        // CrossFade để chuyển mượt, sau đó set normalizedTime
        animator.CrossFade("Running", 0.2f);
        animator.Play("Running", 0, normalizedTime);
    }
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