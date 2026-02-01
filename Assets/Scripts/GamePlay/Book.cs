using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Book : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
        if(collision.CompareTag("Player"))
        {
            OnKillPlayer();
            Debug.Log("dead");
        }
    }
    private void OnKillPlayer()
    {
        if ( PlayerManager.Instance.isUnDead )
        {
            return;
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        CameraManager.Instance.ArrangeAllSubCameras();
    }
}
