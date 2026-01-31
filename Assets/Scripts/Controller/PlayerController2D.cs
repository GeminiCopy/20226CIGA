using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float acceleration = 50f;
    public float deceleration = 30f;
    
    [Header("跳跃设置")]
    public float jumpForce = 10f;
    public int maxJumpCount = 1;
    public float groundCheckDistance = 0.1f;
    
    [Header("输入设置")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    
    private Rigidbody2D rb;
    private CapsuleCollider2D boxCollider;
    private float moveInput;
    private int currentJumpCount;
    public bool isGrounded;
    private bool isJumping;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        rb.gravityScale = 0.8f;
    }
    
    void Update()
    {
        HandleInput();
        CheckGround();
        UpdateJumpCount();
    }
    
    void FixedUpdate()
    {
        Move();
    }
    
    void HandleInput()
    {
        moveInput = 0f;
        
        if (Input.GetKey(leftKey))
        {
            moveInput = -1f;
        }
        if (Input.GetKey(rightKey))
        {
            moveInput = 1f;
        }
        
        if (Input.GetKeyDown(jumpKey))
        {
            TryJump();
        }
    }
    
    void CheckGround()
    {
        Vector2 bottomCenter = boxCollider.bounds.center - new Vector3(0, boxCollider.bounds.extents.y, 0);
        
        var centerHit = Physics2D.RaycastAll(bottomCenter, Vector2.down, groundCheckDistance);
        isGrounded = false;
        for (int i = 0; i < centerHit.Length; i++)
        {
            var hit = centerHit[i];
            if(hit.collider != null && hit.collider.GetComponent<Ground>() != null)
            {
                isGrounded = true;
                return;
            }
        }
    }
    
    void UpdateJumpCount()
    {
        if (isGrounded && !isJumping)
        {
            currentJumpCount = maxJumpCount;
        }
    }
    
    void TryJump()
    {
        if (currentJumpCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            currentJumpCount--;
            isJumping = true;
        }
    }
    
    void Move()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;
        
        if (Mathf.Abs(speedDiff) > 0.1f)
        {
            float rate = speedDiff > 0 ? acceleration : deceleration;
            rate = Mathf.Abs(speedDiff) < 1f ? deceleration * 2f : rate;
            rb.AddForce(Vector2.right * speedDiff * rate);
        }
        
        float maxSpeed = Mathf.Abs(moveSpeed);
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
        
        if (Mathf.Approximately(moveInput, 0f) && Mathf.Abs(rb.velocity.x) < 0.1f)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Ground>() != null)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isJumping = false;
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// 设置移动速度
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }
    
    /// <summary>
    /// 获取当前移动速度
    /// </summary>
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    
    /// <summary>
    /// 设置跳跃是否启用
    /// </summary>
    public void SetJumpEnabled(bool enabled)
    {
        if (!enabled)
        {
            currentJumpCount = 0;
        }
        else if (isGrounded)
        {
            currentJumpCount = maxJumpCount;
        }
    }
    
    /// <summary>
    /// 强制设置跳跃次数
    /// </summary>
    public void SetJumpCount(int jumpCount)
    {
        maxJumpCount = jumpCount;
        if (isGrounded)
        {
            currentJumpCount = jumpCount;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2 bottomCenter = boxCollider.bounds.center - new Vector3(0, boxCollider.bounds.extents.y, 0);
            Vector2 leftPoint = bottomCenter - new Vector2(boxCollider.bounds.extents.x, 0);
            Vector2 rightPoint = bottomCenter + new Vector2(boxCollider.bounds.extents.x, 0);
            
            // 绘制地面检测射线
            Gizmos.DrawLine(bottomCenter, bottomCenter + Vector2.down * groundCheckDistance);
            Gizmos.DrawLine(leftPoint, leftPoint + Vector2.down * groundCheckDistance);
            Gizmos.DrawLine(rightPoint, rightPoint + Vector2.down * groundCheckDistance);
        }
    }
}
