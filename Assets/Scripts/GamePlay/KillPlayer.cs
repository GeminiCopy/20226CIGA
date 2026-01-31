using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
//���ص�ɱ����ҵ���ײ����
public class KillPlayer : MonoBehaviour
{
    public DeathWay deathWay = DeathWay.None;
    public enum DeathWay
    {
        None,
        Bury
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9)
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
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        CameraManager.Instance.Clear();
        //GameManager.Instance.OnDie();
    }
}