using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public GameObject[] levelPrefabs; // Danh sách prefab màn chơi
    private GameObject currentLevel;
    private int currentLevelIndex = 0;
    public PrettyCloneSpawner prettyCloneSpawner; // Gán sẵn trên LevelManager
    public GameObject EndJS;
    public GroupPlayerMovement GroupPlayerMovement; // Gán sẵn trên LevelManager

    private void Start()
    {
        LoadLevel(currentLevelIndex); // Bắt đầu game với level 0

    }

    public void LoadLevel(int index)
    {
        // Huỷ màn cũ nếu có
        if (currentLevel != null)
            Destroy(currentLevel);

        // Kiểm tra hợp lệ
        if (index >= 0 && index < levelPrefabs.Length)
        {
            currentLevelIndex = index;

            // Instantiate màn mới
            currentLevel = Instantiate(levelPrefabs[index]);

            Debug.Log($"[LEVEL] Loaded Level {index}");

            // Gọi Init ngay sau khi load xong (update luôn)
            InitGame();
        }
        else
        {
            Debug.LogWarning("Invalid level index: " + index);
        }
    }

    public void InitGame()
    {
        Debug.Log($"[INIT] Init Level {currentLevelIndex}");

        // Spawn clone ngay lập tức
        prettyCloneSpawner.SpawnClones();

        // Có thể thêm: reset score, reset UI, hiệu ứng, ...
    }


    public void LoadNextLevel()
    {
        int nextIndex = currentLevelIndex + 1;

        if (nextIndex < levelPrefabs.Length)
        {
            LoadLevel(nextIndex); // Load mới tự động Init luôn
        }
        else
        {
            Debug.Log("[LEVEL] All levels complete!");
            EndGame();
        }
    }

    public void EndGame()
    {
        Debug.Log("[GAME] Game Ended");

    
        //
        foreach (var clone in prettyCloneSpawner.clones)
        {
            CloneAnimatorController anim = clone.GetComponent<CloneAnimatorController>();
            if (anim != null)
                anim.PlayDancing();  // Hoặc gọi SetState(Running) tùy thiết kế
        }
        GroupPlayerMovement.StopMovement();

       
    }
    public void StartGame()
    {
    foreach (var clone in prettyCloneSpawner.clones)
    {
        CloneAnimatorController anim = clone.GetComponent<CloneAnimatorController>();
        if (anim != null)
            anim.PlayRunning();  // Hoặc gọi SetState(Running) tùy thiết kế
    }
    }

    public int GetCurrentLevel() => currentLevelIndex;
}
