using UnityEngine;

public class BookSpawner : MonoBehaviour
{
    [Header("组件引用")]
    public GameObject SpawnPoint; // 实际生成点的定位器
    private GameObject bookPrefeb;

    [Header("生成设置")]
    public string bookPath = "Prefebs/Book";
    public float SpawnTime = 0.5f;
    private float spawnCounter;

    [Header("移动设置")]
    public float moveSpeed = 3f;  // 移动速度
    public float startX = 11f;    // 起始X坐标（右侧）
    public float endX = -4f;      // 结束X坐标（左侧）
    public float fixedY = 10f;    // 固定的Y轴高度

    private void Awake()
    {
        // 1. 资源加载（增加了防空保护）
        bookPrefeb = ResourcesMgr.Instance != null ? ResourcesMgr.Instance.Load<GameObject>(bookPath) :
            // 如果单例未初始化，尝试直接加载
            Resources.Load<GameObject>(bookPath);

        spawnCounter = SpawnTime;

        // 2. 初始化位置
        transform.position = new Vector3(startX, fixedY, transform.position.z);
    }

    private void Update()
    {
        HandleMovement();
        HandleSpawning();
    }

    /// <summary>
    /// 处理移动逻辑：从 startX 移动到 endX，然后循环
    /// </summary>
    private void HandleMovement()
    {
        // 向左移动
        transform.Translate(Vector3.left*(moveSpeed*Time.deltaTime));

        // 如果超出了左边界 (-4)，瞬间回到右边界 (11)
        if (transform.position.x < endX)
        {
            transform.position = new Vector3(startX, fixedY, transform.position.z);
        }
    }

    /// <summary>
    /// 处理生成逻辑
    /// </summary>
    private void HandleSpawning()
    {
        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            spawnCounter = SpawnTime;
            SpawnBook();
        }
    }

    public void SpawnBook()
    {
        if (bookPrefeb == null) return;

        // 计算随机生成位置
        // 如果 left 和 right 是子物体，它们会跟随主物体移动，Random.Range 会在当前移动到的区域内取值
        float currentLeftX = transform.position.x - Random.value;
        float currentRightX = transform.position.x + Random.value;
        
        float spawnX = Random.Range(currentLeftX, currentRightX);

        // 设置生成点位置（保持原有的Y和Z）
        Vector3 finalSpawnPos = new Vector3(spawnX, transform.position.y, transform.position.z);

        // 如果有指定的SpawnPoint对象，也可以更新它的位置（为了调试可视化）
        if (SpawnPoint != null)
        {
            SpawnPoint.transform.position = finalSpawnPos;
        }

        // 生成书本
        Instantiate(bookPrefeb, finalSpawnPos, Quaternion.identity);
    }
}