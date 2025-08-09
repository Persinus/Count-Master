using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class GroupPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float dragSensitivity = 0.01f;

    public float minX = 2f;
    public float maxX = 7f;

    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    private bool hasStarted = false;

    private Rigidbody rb;
    private float targetX;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        targetX = transform.position.x;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // ⛔ Nếu bấm vào UI -> không bắt đầu
                if (IsPointerOverUI(touch.position))
                    return;

                isDragging = true;
                lastTouchPosition = touch.position;
                hasStarted = true;
                LevelManager.Instance.StartGame();
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                float deltaX = (touch.position.x - lastTouchPosition.x) * dragSensitivity;
                targetX += deltaX;
                targetX = Mathf.Clamp(targetX, minX, maxX);
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!hasStarted) return;

        Vector3 currentPos = rb.position;
        Vector3 targetPos = new Vector3(targetX, currentPos.y, currentPos.z + moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(targetPos);
    }

    public void StopMovement()
    {
        hasStarted = false;
        isDragging = false;
    }

    // ✅ Check bấm UI
    bool IsPointerOverUI(Vector2 pos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = pos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
