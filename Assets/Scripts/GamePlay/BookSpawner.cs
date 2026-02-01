using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSpawner : MonoBehaviour
{
    public Transform left;
    public Transform right;
    public GameObject SpawnPoint;
    public GameObject bookPrefeb;
    public float SpawnCounter, SpawnTime;
    public string bookPath= "Prefebs/Book";
    private void Awake()
    {
        left = transform.Find("Left").transform;
        left = transform.Find("Right").transform;
        SpawnPoint = transform.Find("Right").gameObject;
        bookPrefeb = ResourcesMgr.Instance.Load<GameObject>(bookPath);
        SpawnTime=0.5f;
        SpawnCounter = SpawnTime;
    }
    private void Update()
    {
        SpawnCounter -= Time.deltaTime;
        if (SpawnCounter < 0)
        {
            SpawnCounter = SpawnTime;
            SpawnBook();
        }
    }
    public void SpawnBook()
    {
        float spawnX=Random.Range(left.transform.position.x, right.transform.position.x);
        SpawnPoint.transform.position =new Vector3 (spawnX,SpawnPoint.transform.position.y, SpawnPoint.transform.position.z);
        Instantiate(bookPrefeb).transform.position=SpawnPoint.transform.position;
           
    }
}
