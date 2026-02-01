using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
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
        if(collision.CompareTag("Player"))
        {
            OnKillPlayer();
            Debug.Log("dead");
        }
    }
    public void OnKillPlayer()
    {
        switch(deathWay)
        {
            case DeathWay.None:
                break;
            case DeathWay.Bury:
                //������������
                break;
            case DeathWay.Falling:
                break;
        }
        GameManager.Instance.OnDie();
        StartCoroutine(DeathSequence());
        
    }
    IEnumerator DeathSequence()
    {
        Debug.Log("玩家死亡");

        // 等待3秒
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        CameraManager.Instance.Clear();
        Debug.Log("3秒后执行复活逻辑");
    }
}