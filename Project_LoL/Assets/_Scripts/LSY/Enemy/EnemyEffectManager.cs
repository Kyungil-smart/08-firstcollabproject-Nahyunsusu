using DG.Tweening;
using UnityEngine;

public class EnemyEffectManager : MonoBehaviour
{
    private SpriteRenderer _renderer;

    private float _hitEffectDuration = 0.2f;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void PlayHitEffect()
    {
        if (_renderer == null) return;

        Color original = _renderer.color;
        _renderer.color = new Color(0.5f, original.g, original.b, original.a);
        _renderer.DOColor(original, _hitEffectDuration).SetEase(Ease.InCubic);
    }
}