using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
//挂载到杀死玩家的碰撞体上
public class KillPlayer : MonoBehaviour
{
    public DeathWay deathWay = DeathWay.None;
    public enum DeathWay
    {
        None,
        Bury,
        Falling
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        OnKillPlayer();
    }
    public void OnKillPlayer()
    {
        switch(deathWay)
        {
            case DeathWay.None:
                break;
            case DeathWay.Bury:
                //播放死亡动画
                break;
            case DeathWay.Falling:
                break;
        }
        GameManager.Instance.OnDie();
    }
}
