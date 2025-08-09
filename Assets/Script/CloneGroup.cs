using UnityEngine;
using TMPro;
using DG.Tweening;

public class BubbleNumberClone : MonoBehaviour
{
    private TextMeshProUGUI text;

    [SerializeField] private GameObject bubblePrefab;

    private int currentCount = 0;

    void Start()
    {
        bubblePrefab.SetActive(true);
        text = bubblePrefab.GetComponentInChildren<TextMeshProUGUI>();
        UpdateCount();
    }

    public void UpdateCount()
    {
        if (text == null) return;

        int liveCount = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != null && child.gameObject.activeSelf)
                liveCount++;
        }
        int realCount = Mathf.Max(0, liveCount - 1);

        // Tween số từ currentCount đến realCount trong 0.5s
        DOTween.To(() => currentCount, x => {
            currentCount = x;
            text.text = currentCount.ToString();
        }, realCount, 1f).SetEase(Ease.OutCubic);
    }
}
