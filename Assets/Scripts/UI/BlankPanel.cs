using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BlankPanel : BasePanel
{
    public float fadeTime = .5f;
    public Image panelBg;
    public event Action OnFadeInComplete;
    public DialogController myBubble;
    protected override void Awake()
    {
        base.Awake();
        DialogManager.Inst.Register("Blank",myBubble);
    }
    private void OnDestroy()
    {
        if (DialogManager.Inst != null)
        {
            DialogManager.Inst.UnRegister("Blank");
        }
    }
    public override void ShowMe()
    {
        FadeIn().ContinueWith((() =>
        {
            OnFadeInComplete?.Invoke();
            OnFadeInComplete = null;
        })).Forget();
    }

    public override void HideMe()
    {
        FadeOut().Forget();
    }
    
    public async UniTask FadeOut()
    {
        float timer = 0;
        Color color = panelBg.color; 

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            color.a = 1 - (timer / fadeTime);
            panelBg.color = color;

            await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        }

        color.a = 0;
        panelBg.color = color;
    }

    public async UniTask FadeIn()
    {
        float timer = 0;
        Color color = panelBg.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            color.a = timer / fadeTime;
            panelBg.color = color;

            await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
        }

        color.a = 1;
        panelBg.color = color;
    }
}