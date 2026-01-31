using UnityEngine;

public class ColliderToggle : MonoBehaviour
{
    // 状态选项枚举
    public enum ToggleState
    {
        Disappear,  // 消失状态
        Appear      // 出现状态
    }
    
    [Header("目标组件")]
    public Collider2D targetCollider;
    public Collider2D selfCollider;
    
    [Header("状态设置")]
    public ToggleState defaultState = ToggleState.Disappear;
    
    // 内部状态变量
    private bool isActivated = false;
    
    void Awake()
    {
        Deactivate();
    }
    
    public void Activate()
    {
        Debug.Log("LOG:ACTIVATE");
        // 无论什么状态都要设置isActivated
        isActivated = true;
        
        if (defaultState == ToggleState.Appear)
        {
            ShowAll();
        }
        else if (defaultState == ToggleState.Disappear)
        {
            HideAll();
        }
    }
    
    public void Deactivate()
    {
         Debug.Log("LOG:DEACTIVATE");
        // 无论什么状态都要设置isActivated
        isActivated = false;
        
        if (defaultState == ToggleState.Appear)
        {
            HideAll();
        }
        else if (defaultState == ToggleState.Disappear)
        {
            ShowAll();
        }
    }

    private void ShowAll()
    {
        targetCollider.enabled = true;
        selfCollider.excludeLayers = 0;
    }

    private void HideAll()
    {
        targetCollider.enabled = false;
        selfCollider.excludeLayers = 1 << 9;
    }
    
    // 获取当前激活状态
    public bool IsActivated()
    {
        return isActivated;
    }
}
