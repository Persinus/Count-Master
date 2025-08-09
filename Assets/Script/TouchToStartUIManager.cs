using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TouchToStartUIManager : MonoBehaviour
{
    public GameObject startUI;
    private bool hasStarted = false;

    void Update()
    {
        if (hasStarted) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleClick(Input.GetTouch(0).position);
        }
#endif
    }

    void HandleClick(Vector2 screenPos)
    {
        // Nếu chạm vào UI thì thoát luôn, không làm gì
        if (IsPointerOverUI(screenPos))
            return;

        // Nếu chạm vùng xanh -> tắt UI và bắt đầu game
        startUI.SetActive(false);
        hasStarted = true;
        LevelManager.Instance.StartGame();
    }

    bool IsPointerOverUI(Vector2 pos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = pos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
}
