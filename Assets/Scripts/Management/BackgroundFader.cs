using System;
using UnityEngine;
using DG.Tweening;

public class BackgroundFader : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] bg1;
    [SerializeField] private float fadeDuration = 3f;
    
    
    void Start()
    {
        // Infinite Crossfade Loop
        Sequence crossFade = DOTween.Sequence();
        crossFade.Append(FadeGroup(bg1, 0f, fadeDuration));
        crossFade.Append(FadeGroup(bg1, 1f, fadeDuration));
        crossFade.SetLoops(-1);
    }
    
    
    private Tween FadeGroup(SpriteRenderer[] group, float alpha, float duration)
    {
        // Returns a tween that affects all sprites in the group simultaneously
        return DOTween.To(() => group[0].color.a, x => {
            foreach(var s in group) s.color = new Color(1,1,1,x);
        }, alpha, duration).SetId("backgroundFader");
    }

    private void OnDestroy()
    {
        DOTween.Kill("backgroundFader");
    }
}