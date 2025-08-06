using UnityEngine;

public class CloneTriggerHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            // Gọi lại object pool để return về
            ObjectPooler.Instance.ReturnToPool(gameObject);
            // Thông báo cho CloneGroup cập nhật số lượng
            CloneGroup cloneGroup = GetComponentInParent<CloneGroup>();
            if (cloneGroup != null)
            {
                cloneGroup.UpdateCount();
            }
            else
            {
                Debug.LogWarning("CloneGroup không được tìm thấy trong parent của clone.");
            }
        }
    }
}
