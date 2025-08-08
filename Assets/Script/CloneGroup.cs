using UnityEngine;
using TMPro;

public class BubbleNumberClone : MonoBehaviour
{
    private GameObject bubbleInstance;
    private TextMeshProUGUI text;

    [SerializeField] private GameObject bubblePrefab;

    void Start()
    {
        if (bubblePrefab == null)
        {
            Debug.LogWarning("⚠️ Bubble prefab chưa được gán!");
            return;
        }

        bubbleInstance = Instantiate(bubblePrefab, transform);
        bubbleInstance.transform.localPosition = new Vector3(0f, 37f, 0);
        bubbleInstance.transform.localRotation = Quaternion.Euler(-30f, 0, 0);
        bubbleInstance.transform.localScale = Vector3.one * 0.5f;

        text = bubbleInstance.GetComponentInChildren<TextMeshProUGUI>();
        UpdateCount();
    }

    public void UpdateCount()
    {
        if (text == null) return;

        int liveCount = 0;

        // Đếm số lượng clone thật sự còn sống trong cloneParent
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null && child.gameObject.activeSelf)
                liveCount++;
        }
        int realCount = liveCount - 1;
        text.text = realCount.ToString();
    }
}
