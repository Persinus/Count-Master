using UnityEngine;

public class TouchToStartUIManager : MonoBehaviour
{
    public GameObject startUI;
    private bool hasStarted = false;

    void Update()
    {
        if (hasStarted) return;

        if (Input.GetMouseButtonDown(0)) // chạm hoặc click
        {
            startUI.SetActive(false); // ẩn UI
            hasStarted = true;

           
        }
       
    }

}
