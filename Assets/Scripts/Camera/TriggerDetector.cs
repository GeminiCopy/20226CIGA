using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class TriggerDetector : MonoBehaviour
{
    [SerializeField] private bool playerDetected;
    
    // 公共属性，暴露playerDetected状态
    public bool IsPlayerDetected => playerDetected;

    //public event System.Action<Collider2D> OnObjectDetected;
    private HashSet<Collider2D> storedColliders = new HashSet<Collider2D>();
    //public IEnumerable<Collider2D> StoredColliders => storedColliders;
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ENTER:" + other.name);
        //OnObjectDetected?.Invoke(other);
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("玩家进入");
            PlayerManager.Instance.isUnDead++;
            SetPlayerDetected(true);
            ActivateAllStoredColliders();
        }

        ColliderToggle component = other.GetComponent<ColliderToggle>();
        if (component != null)
        {
            Debug.Log("collidetoggle进入:" + component.gameObject.name);
            storedColliders.Add(other);
        }
        
        if (!playerDetected)
        {
            return;
        }
        
        if (component != null)
        {
            component.Activate();
            Debug.Log(component.gameObject.name + "激活");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("玩家离开");
            PlayerManager.Instance.isUnDead--;
            SetPlayerDetected(false);
            DeactivateAllStoredColliders();
            return;
        }

        ColliderToggle component = other.GetComponent<ColliderToggle>();
        if(component != null)
        {
            Debug.Log("collidetoggle离开:" + component.gameObject.name);
            storedColliders.Remove(other);
            component.Deactivate();
            Debug.Log(component.gameObject.name + "失活");
        }
    }
    
    void ActivateAllStoredColliders()
    {
        foreach (Collider2D collider in storedColliders)
        {
            if (collider != null)
            {
                ColliderToggle toggle = collider.GetComponent<ColliderToggle>();
                if (toggle != null)
                {
                    Debug.Log(toggle.gameObject.name + "激活");
                    toggle.Activate();
                }
            }
        }
    }
    
    void DeactivateAllStoredColliders()
    {
        foreach (Collider2D collider in new HashSet<Collider2D>(storedColliders))
        {
            if (collider != null)
            {
                ColliderToggle toggle = collider.GetComponent<ColliderToggle>();
                if (toggle != null)
                {
                    Debug.Log(toggle.gameObject.name + "失活");
                    toggle.Deactivate();
                }
            }
        }
    }
    
    /// <summary>
    /// 设置玩家检测状态，并通知CameraManager
    /// </summary>
    private void SetPlayerDetected(bool detected)
    {
        if (playerDetected != detected)
        {
            playerDetected = detected;
            
            // 通知CameraManager状态变化
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.OnTriggerDetectorStateChanged(this, detected);
            }
        }
    }
}
