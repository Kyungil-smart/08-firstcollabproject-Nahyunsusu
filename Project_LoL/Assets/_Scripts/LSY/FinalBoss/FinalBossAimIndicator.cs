using UnityEngine;
using System;
using System.Collections;

public class FinalBossAimIndicator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Vector2        _lastFacingDir = Vector2.right;

    public Vector2 lastFacingDir => _lastFacingDir;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(float scaleX, float scaleY, Sprite sprite = null)
    {
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
        if (sprite != null) _spriteRenderer.sprite = sprite;
    }

    public void StartTracking(Transform target, float trackDuration, Action onTrackEnd)
    {
        StartCoroutine(TrackRoutine(target, trackDuration, onTrackEnd));
    }

    private IEnumerator TrackRoutine(Transform target, float duration, Action onTrackEnd)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (target != null)
            {
                Vector2 dir = (Vector2)target.position - (Vector2)transform.position;
                if (dir.sqrMagnitude > 0.001f)
                {
                    _lastFacingDir = dir.normalized;
                    float angle    = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        onTrackEnd?.Invoke();
    }

    public void SelfDestroy(float delay = 0f)
    {
        Destroy(gameObject, delay);
    }
}
