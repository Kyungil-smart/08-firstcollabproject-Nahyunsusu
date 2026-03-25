using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffectManager : MonoBehaviour
{
    private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();

    private const float HIT_EFFECT_DURATION = 0.2f;

    private void Awake()
    {
        GetComponentsInChildren(true, _renderers);
    }

    public void PlayHitEffect()
    {
        if (_renderers.Count == 0) return;
        StartCoroutine(HitColorRoutine());
    }

    private IEnumerator HitColorRoutine()
    {
        List<Color> originals = new List<Color>();
        foreach (SpriteRenderer Sprite in _renderers)
            originals.Add(Sprite.color);

        foreach (SpriteRenderer Sprite in _renderers)
            Sprite.color = new Color(0.5f, 0f, 0f, Sprite.color.a);

        yield return new WaitForSeconds(HIT_EFFECT_DURATION);

        for (int i = 0; i < _renderers.Count; i++)
            _renderers[i].color = originals[i];
    }
}