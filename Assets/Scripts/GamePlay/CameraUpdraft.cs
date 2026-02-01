using UnityEngine;

/// <summary>
/// 相机检测上升气流控制脚本
/// </summary>
public class CameraUpdraft : MonoBehaviour
{
    [Header("检测组件")] [Tooltip("将带有 TriggerDetector 脚本的相机或物体拖到这里")]
    public TriggerDetector targetCameraDetector;

    [Header("气流参数")] [Tooltip("上升气流的施加力大小")]
    public float updraftForce = 30f;

    [Tooltip("最大上升速度限制（防止飞出地图）")] public float maxUpwardSpeed = 10f;

    // 缓存玩家刚体引用
    private Rigidbody2D rb1; // 表世界玩家
    private Rigidbody2D rb2; // 里世界玩家

    private void Start()
    {
        // 1. 从 PlayerManager 单例中获取两个玩家的引用
        if (PlayerManager.Instance != null)
        {
            // 获取表世界玩家刚体
            if (PlayerManager.Instance.player1 != null)
            {
                rb1 = PlayerManager.Instance.player1.GetComponent<Rigidbody2D>();
            }

            // 获取里世界玩家刚体
            if (PlayerManager.Instance.player2 != null)
            {
                rb2 = PlayerManager.Instance.player2.GetComponent<Rigidbody2D>();
            }
        }
        else
        {
            Debug.LogWarning("CameraUpdraft: 未找到 PlayerManager 实例，无法获取玩家引用！");
        }
    }

    private void FixedUpdate()
    {
        // 2. 检查 TriggerDetector 是否检测到了玩家
        if (targetCameraDetector != null && targetCameraDetector.IsPlayerDetected)
        {
            // 3. 如果检测到，给两个玩家都施加向上的力（保持同步）
            if (rb1.position.x is < -7 and > -14)
            {
                ApplyUpdraft(rb2); // 里世界玩家
                ApplyUpdraft(rb1); // 表世界玩家
            }
        }
        else
        {
            targetCameraDetector = FindAnyObjectByType<TriggerDetector>(FindObjectsInactive.Include);
        }
    }

    /// <summary>
    /// 对指定的刚体施加上升气流
    /// </summary>
    /// <param name="rb">目标刚体</param>
    private void ApplyUpdraft(Rigidbody2D rb)
    {
        if (rb == null) return;

        // 施加向上的力 (ForceMode2D.Force 适合持续的力，如风力)
        rb.AddForce(Vector2.up*updraftForce, ForceMode2D.Force);

        // 限制Y轴最大速度，获得更平滑的悬浮感
        if (rb.velocity.y > maxUpwardSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxUpwardSpeed);
        }
    }
}