using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemDescVision : MonoBehaviour
{
    public int id;
    public TextMeshPro tmp;
    private ItemDesc desc;
    private void Awake()
    {
        desc = ItemManager.Inst.Get(id);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        tmp.gameObject.SetActive(true);
        if (other.gameObject.name.Contains("Camera"))
        {
            tmp.text = desc.desc;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        tmp.gameObject.SetActive(false);
    }
}