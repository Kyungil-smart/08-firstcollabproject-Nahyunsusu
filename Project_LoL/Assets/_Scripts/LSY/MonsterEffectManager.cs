using System.Collections;
using UnityEngine;

public class MonsterEffectManager : MonoBehaviour
{
    private SpriteRenderer[] _spriteRenderers;
    private Color[] _originColors;

    private void Awake()
    {
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        
        _originColors = new Color[_spriteRenderers.Length];

        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            if (_spriteRenderers[i] != null)
                _originColors[i] = _spriteRenderers[i].color;
        }
    }

    public void PlayHitEffect()
    {
        PlayHitFlash();
    }

    public void PlayHitFlash()
    {
        if (_spriteRenderers == null || _spriteRenderers.Length == 0) return;
        StopAllCoroutines();
        StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            if (_spriteRenderers[i] != null)
                _spriteRenderers[i].color = Color.red; 
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            if (_spriteRenderers[i] != null)
                _spriteRenderers[i].color = _originColors[i];
        }
    }

    public void RestoreColors()
    {
        if (_spriteRenderers == null) return;
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            if (_spriteRenderers[i] != null)
                _spriteRenderers[i].color = _originColors[i];
        }
    }
}