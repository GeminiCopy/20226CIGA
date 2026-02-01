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

    // 添加状态跟踪
    private bool _isAnimating = false;
    private Coroutine _currentAnimation;
    private float _targetAlpha = 0f; // 目标透明度

    protected override void Awake()
    {
        base.Awake();
        DialogManager.Inst.Register("Blank", myBubble);

        // 初始化时设置完全透明
        if (panelBg != null)
        {
            Color color = panelBg.color;
            color.a = 0f;
            panelBg.color = color;
        }
    }

    private void Start()
    {
        // 不调用 HideMe()，而是直接设置透明
        // 这样当需要显示时，会从透明渐变到不透明
    }

    private void OnDestroy()
    {
        // 停止所有动画
        if (_currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
        }

        if (DialogManager.Inst != null)
        {
            DialogManager.Inst.UnRegister("Blank");
        }
    }

    public override void ShowMe()
    {
        Debug.Log("BlankPanel: ShowMe 被调用");

        // 确保对象激活
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // 停止正在进行的动画
        if (_isAnimating && _currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
        }

        // 开始渐变到不透明（黑屏）
        _currentAnimation = StartCoroutine(FadeToAlpha(1f));
    }

    public override void HideMe()
    {
        Debug.Log("BlankPanel: HideMe 被调用");

        // 停止正在进行的动画
        if (_isAnimating && _currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
        }

        // 开始渐变到透明
        _currentAnimation = StartCoroutine(FadeToAlpha(0f));
    }

    /// <summary>
    /// 渐变到指定透明度
    /// </summary>
    private IEnumerator FadeToAlpha(float targetAlpha)
    {
        _isAnimating = true;
        _targetAlpha = targetAlpha;

        // 获取当前透明度
        float startAlpha = panelBg.color.a;
        float elapsedTime = 0f;

        Debug.Log($"开始渐变: {startAlpha} -> {targetAlpha}, 时间: {fadeTime}");

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeTime);
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            // 设置透明度
            SetAlpha(currentAlpha);

            yield return null;
        }

        // 确保达到目标值
        SetAlpha(targetAlpha);
        _isAnimating = false;

        Debug.Log($"渐变完成: 透明度 = {targetAlpha}");

        // 如果是显示完成的回调
        if (targetAlpha >= 0.99f) // 近似完全显示
        {
            OnFadeInComplete?.Invoke();
            OnFadeInComplete = null;
        }
    }

    /// <summary>
    /// 立即设置透明度（无渐变）
    /// </summary>
    public void SetAlphaImmediate(float alpha)
    {
        if (_isAnimating && _currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
            _isAnimating = false;
        }

        SetAlpha(alpha);
        _targetAlpha = alpha;
    }

    /// <summary>
    /// 设置面板透明度
    /// </summary>
    private void SetAlpha(float alpha)
    {
        if (panelBg != null)
        {
            Color color = panelBg.color;
            color.a = Mathf.Clamp01(alpha);
            panelBg.color = color;

            // 根据透明度控制raycast
            panelBg.raycastTarget = alpha > 0.01f;
        }
    }

    /// <summary>
    /// 强制显示（立即黑屏）
    /// </summary>
    public void ShowImmediate()
    {
        if (_currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
            _isAnimating = false;
        }

        SetAlpha(1f);
        _targetAlpha = 1f;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // 立即触发回调
        OnFadeInComplete?.Invoke();
        OnFadeInComplete = null;
    }

    /// <summary>
    /// 强制隐藏（立即透明）
    /// </summary>
    public void HideImmediate()
    {
        if (_currentAnimation != null)
        {
            StopCoroutine(_currentAnimation);
            _isAnimating = false;
        }

        SetAlpha(0f);
        _targetAlpha = 0f;
    }

    /// <summary>
    /// 检查是否正在显示中
    /// </summary>
    public bool IsShowing()
    {
        return panelBg != null && panelBg.color.a > 0.01f;
    }

    /// <summary>
    /// 等待渐变完成
    /// </summary>
    public IEnumerator WaitForFadeComplete()
    {
        while (_isAnimating)
        {
            yield return null;
        }
    }
}