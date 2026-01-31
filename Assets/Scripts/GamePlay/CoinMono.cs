using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinMono : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // 触发器检测 - 与Player碰撞时触发
    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞对象是否是Player layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log($"Coin被玩家拾取，添加新摄像机");
            
            // 添加子摄像机
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.AddNewSubCamera();
            }
            
            // 销毁自身
            Destroy(gameObject);
        }
    }
}
