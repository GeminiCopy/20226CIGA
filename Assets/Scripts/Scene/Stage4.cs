using UnityEngine;

public class Stage4 : MonoBehaviour
{
    public GameObject innerScene;

    public GameObject outerScene;
    // Start is called before the first frame update
    void Start()
    {
        CameraManager.Instance.Clear();
        UIMgr.Instance.ShowPanel<BlankPanel>(callback: OnStage4Start);
        PlayerManager.Instance.player1.CanMove = false;
        PlayerManager.Instance.player2.CanMove = false;
    }
    private void OnStage4Start(BlankPanel bp)
    {
        DialogManager.Inst.Load("tbstg4dialog");
        bp.OnFadeInComplete += () =>
        {
            DialogManager.Inst.Play(onComplete: () =>
            {
                UIMgr.Instance.HidePanel<BlankPanel>();
                DialogManager.Inst.Play(6,
                    (() =>
                    {
                        PlayerManager.Instance.player1.CanMove = true;
                        PlayerManager.Instance.player2.CanMove = true;
                    }));
            });
        };
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("tri");

        if (other.CompareTag("Player"))
        {
            Debug.Log("suc");
            innerScene.SetActive(false);
            PlayerManager.Instance.player1.GetComponent<SpriteRenderer>().enabled = true;
            PlayerManager.Instance.player2.GetComponent<SpriteRenderer>().enabled = false;
            outerScene.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("tri");

            if (other.transform.position.y > -8)
            {
                outerScene.SetActive(false);
                PlayerManager.Instance.player1.GetComponent<SpriteRenderer>().enabled = false;
                PlayerManager.Instance.player2.GetComponent<SpriteRenderer>().enabled = true;
                innerScene.SetActive(true);
            }
            else
            {
                UIMgr.Instance.ShowPanel<BlankPanel>(callback: OnStage4End);
            }
        }
    }
    private void OnStage4End(BlankPanel bp)
    {
        bp.OnFadeInComplete += () =>
        {
            DialogManager.Inst.Play(onComplete: () =>
            {
                SceneMgr.Instance.LoadSceneAsyn("StartScene",
                    (() => { UIMgr.Instance.HidePanel<BlankPanel>(); }));
            });
        };
    }
}