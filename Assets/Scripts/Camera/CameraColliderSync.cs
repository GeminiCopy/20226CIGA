using UnityEngine;

public class CameraColliderSync : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private Camera cam;
    private float lastAspect;
    
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        cam = GetComponentInChildren<Camera>();
        
        if (boxCollider == null)
        {
            Debug.LogWarning("CameraColliderSync: No BoxCollider2D found on this object");
        }
        
        if (cam == null)
        {
            Debug.LogWarning("CameraColliderSync: No Camera found in children");
        }
    }
    
    void Update()
    {
        if (boxCollider == null || cam == null) return;
        
        float currentAspect = (float)Screen.width / Screen.height;
        
        if (cam.orthographicSize != boxCollider.size.y || Mathf.Abs(currentAspect - lastAspect) > 0.001f)
        {
            boxCollider.size = new Vector2(
                cam.orthographicSize * 2f * currentAspect,
                cam.orthographicSize * 2f
            );
            lastAspect = currentAspect;
        }
    }
}
