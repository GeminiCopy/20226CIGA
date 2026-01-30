using UnityEngine;

public class ColliderToggle : MonoBehaviour
{
    public Collider2D targetCollider;
    public Collider2D selfCollider;
    
    void Awake()
    {
        Deactivate();
    }
    
    public void Activate()
    {
        if (targetCollider != null)
        {
            targetCollider.enabled = true;
        }
        
        if (selfCollider != null)
        {
            selfCollider.isTrigger = false;
        }
    }
    
    public void Deactivate()
    {
        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }
        
        
        if (selfCollider != null)
        {
            selfCollider.isTrigger = true;
        }
    }
}
