using UnityEngine;

public class RandomMeshColor : MonoBehaviour
{
    public GameObject[] objects; // Gán 4 object vào đây

    void Start()
    {
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Color randomColor = Random.ColorHSV(); // Tạo màu ngẫu nhiên đẹp
                renderer.material.color = randomColor;
            }
        }
    }
}
