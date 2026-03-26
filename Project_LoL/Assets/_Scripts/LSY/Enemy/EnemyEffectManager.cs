using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffectManager : MonoBehaviour
{
    private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
    private List<Color> _originalColors = new List<Color>();
    private Coroutine _hitCoroutine;

    private const float HIT_EFFECT_DURATION = 0.2f;

    private void Awake()
    {
        GetComponentsInChildren(true, _renderers);

        // 원래 색상 미리 저장
        foreach (SpriteRenderer sr in _renderers)
            _originalColors.Add(sr.color);
    }

    public void PlayHitEffect()
    {
        if (_renderers.Count == 0) return;

        // 진행 중인 코루틴 있으면 중단하고 색상 복구 후 재시작
        if (_hitCoroutine != null)
        {
            StopCoroutine(_hitCoroutine);
            RestoreColors();
        }

        _hitCoroutine = StartCoroutine(HitColorRoutine());
    }

    // 죽음 시 색상 복구 (SetActive 전에 호출)
    public void RestoreColors()
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            if (_renderers[i] != null)
                _renderers[i].color = _originalColors[i];
        }
    }

    private IEnumerator HitColorRoutine()
    {
        // 빨간색으로 변경
        foreach (SpriteRenderer sr in _renderers)
            sr.color = new Color(0.5f, 0f, 0f, sr.color.a);

        yield return new WaitForSeconds(HIT_EFFECT_DURATION);

        RestoreColors();
        _hitCoroutine = null;
    }
}