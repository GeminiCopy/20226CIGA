using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
//挂载到杀死玩家的碰撞体上
public class KillPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        OnKillPlayer();
    }
    public void OnKillPlayer()
    {
        GameManager.Instance.OnDie();
    }
}
