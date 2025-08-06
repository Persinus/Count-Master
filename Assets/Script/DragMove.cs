using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragMove : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float dragSensitivity = 0.01f;

    public float minX = 2f;
    public float maxX = 7f;

    private Vector2 lastTouchPosition;
    private bool isDragging = false;
    private bool hasStarted = false; // 👈 mới thêm

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
                isDragging = true;
                lastTouchPosition = touch.position;
                hasStarted = true; // 👈 đánh dấu bắt đầu
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
        if (!hasStarted) return; // 👈 Chưa bắt đầu thì không di chuyển

        Vector3 currentPos = rb.position;
        Vector3 targetPos = new Vector3(targetX, currentPos.y, currentPos.z + moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(targetPos);
    }
}
