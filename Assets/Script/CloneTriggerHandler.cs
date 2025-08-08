using UnityEngine;

public class CloneTriggerHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            // Gọi lại object pool để return về
            Singleton<ObjectPool>.Instance.ReturnObject(gameObject);
            // Thông báo cho BubbleNumberClone cập nhật số lượng
            BubbleNumberClone bubbleNumberClone = GetComponentInParent<BubbleNumberClone>();
            if (bubbleNumberClone != null)
            {
                bubbleNumberClone.UpdateCount();
            }
            else
                Debug.LogWarning("BubbleNumberClone không được tìm thấy trong parent của clone.");
        }
        else if (other.CompareTag("Finish"))
        {
            LevelManager.Instance.EndGame();
        }
    }
}
