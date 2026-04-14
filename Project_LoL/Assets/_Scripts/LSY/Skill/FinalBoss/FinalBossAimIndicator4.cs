using UnityEngine;
using System;
using System.Collections;

public class FinalBossLaserAimIndicator : MonoBehaviour
{
    private Vector2 _lastFacingDir = Vector2.right;
    private Transform _bossTransform;
    private float _spawnOffset;
    private float _length;

    public Vector2 lastFacingDir => _lastFacingDir;

    public void Initialize(float length, float thickness, float spawnOffset, Transform bossTransform)
    {
        _length = length;
        _spawnOffset = spawnOffset;
        _bossTransform = bossTransform;

        transform.localScale = new Vector3(length, thickness, 1f);
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
            if (target != null && _bossTransform != null)
            {
                Vector2 dir = (Vector2)target.position - (Vector2)_bossTransform.position;
                if (dir.sqrMagnitude > 0.001f)
                {
                    _lastFacingDir = dir.normalized;
                    float angle = Mathf.Atan2(_lastFacingDir.y, _lastFacingDir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);

                    Vector2 targetPos = (Vector2)_bossTransform.position 
                                      + (_lastFacingDir * _spawnOffset) 
                                      + (_lastFacingDir * (_length * 0.5f));
                                      
                    transform.position = targetPos;
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        onTrackEnd?.Invoke();
    }

    public void SelfDestroy() => Destroy(gameObject);
}