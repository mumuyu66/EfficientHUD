using UnityEngine.UI;
using UnityEngine;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;

public static class DOTweenExtensions
{
    #region MeshImage

    /// <summary>Tweens an Image's color to the given value.
    /// Also stores the image as the tween's target so it can be used for filtered operations</summary>
    /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
    public static TweenerCore<Color, Color, ColorOptions> DOColor(this MeshImage target, Color endValue, float duration)
    {
        TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => target.color, x => target.color = x, endValue, duration);
        t.SetTarget(target);
        return t;
    }

    /// <summary>Tweens an Image's alpha color to the given value.
    /// Also stores the image as the tween's target so it can be used for filtered operations</summary>
    /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
    public static TweenerCore<Color, Color, ColorOptions> DOFade(this MeshImage target, float endValue, float duration)
    {
        TweenerCore<Color, Color, ColorOptions> t = DOTween.ToAlpha(() => target.color, x => target.color = x, endValue, duration);
        t.SetTarget(target);
        return t;
    }

    /// <summary>Tweens an Image's fillAmount to the given value.
    /// Also stores the image as the tween's target so it can be used for filtered operations</summary>
    /// <param name="endValue">The end value to reach (0 to 1)</param><param name="duration">The duration of the tween</param>
    public static TweenerCore<float, float, FloatOptions> DOFillAmount(this MeshImage target, float endValue, float duration)
    {
        if (endValue > 1) endValue = 1;
        else if (endValue < 0) endValue = 0;
        TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.fillAmount, x => target.fillAmount = x, endValue, duration);
        t.SetTarget(target);
        return t;
    }

    /// <summary>Tweens an Image's colors using the given gradient
    /// (NOTE 1: only uses the colors of the gradient, not the alphas - NOTE 2: creates a Sequence, not a Tweener).
    /// Also stores the image as the tween's target so it can be used for filtered operations</summary>
    /// <param name="gradient">The gradient to use</param><param name="duration">The duration of the tween</param>
    public static Sequence DOGradientColor(this MeshImage target, Gradient gradient, float duration)
    {
        Sequence s = DOTween.Sequence();
        GradientColorKey[] colors = gradient.colorKeys;
        int len = colors.Length;
        for (int i = 0; i < len; ++i)
        {
            GradientColorKey c = colors[i];
            if (i == 0 && c.time <= 0)
            {
                target.color = c.color;
                continue;
            }
            float colorDuration = i == len - 1
                ? duration - s.Duration(false) // Verifies that total duration is correct
                : duration * (i == 0 ? c.time : c.time - colors[i - 1].time);
            s.Append(target.DOColor(c.color, colorDuration).SetEase(Ease.Linear));
        }
        return s;
    }

    #endregion
}
