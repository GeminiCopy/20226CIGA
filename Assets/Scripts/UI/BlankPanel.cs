using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlankPanel : BasePanel
{
    public float fadeTime = 1f;
    public Image panelBg;

    void Start()
    {
        // 获取黑色背景
        panelBg = GetComponentInChildren<Image>();

        // 启动时淡出
        StartCoroutine(FadeOut());
    }

    public override void ShowMe()
    {
        StartCoroutine(FadeOut());
    }

    public override void HideMe()
    {
        StartCoroutine(FadeIn());
    }


    IEnumerator FadeOut()
    {
        // 从黑变透明
        float timer = 0;
        Color color = panelBg.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            color.a = 1 - (timer / fadeTime);
            panelBg.color = color;
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        // 从透明变黑
        float timer = 0;
        Color color = panelBg.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            color.a = timer / fadeTime;
            panelBg.color = color;
            yield return null;
        }
    }
}