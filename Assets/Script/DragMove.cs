using UnityEngine;

public class DragMove : MonoBehaviour
{
    private Rigidbody rb;

    public float moveSpeed = 10f;             // tốc độ tiến thẳng
    public float dragSensitivity = 0.05f;     // độ nhạy kéo ngang

    private Vector2 lastTouchPosition;
    private bool isDragging = false;

    [SerializeField] GameObject target;

    void Start()
    {
        rb = target.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        // Luôn chạy thẳng theo Z
        velocity.z = moveSpeed;

        if (isDragging)
        {
            float moveX = (Input.GetTouch(0).position.x - lastTouchPosition.x) * dragSensitivity;

            velocity.x = moveX;
            lastTouchPosition = Input.GetTouch(0).position;
        }
        else
        {
            velocity.x = 0;
        }

        rb.linearVelocity = velocity;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }
}
