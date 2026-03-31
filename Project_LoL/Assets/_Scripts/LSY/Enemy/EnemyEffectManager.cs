using System.Collections;
using UnityEngine;

public class EnemyEffectManager : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Color _originColor;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer != null) _originColor = _spriteRenderer.color;
    }

    public void PlayHitEffect()
    {
        PlayHitFlash();
    }

    public void PlayHitFlash()
    {
        if (_spriteRenderer == null) return;
        StopAllCoroutines();
        StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        _spriteRenderer.color = Color.red; 
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = _originColor;
    }

    public void RestoreColors()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _originColor;
        }
    }
}