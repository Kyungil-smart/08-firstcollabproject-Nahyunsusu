using UnityEngine;
using System;
using System.Collections;

public class FinalBossAimIndicator : MonoBehaviour
{
    private Vector2 _lastFacingDir = Vector2.right;
    private Vector2 _centerPos;
    private float   _radius;
    private bool    _isOrbitSet = false;

    public Vector2 lastFacingDir => _lastFacingDir;

    public void Initialize(float scaleX, float scaleY)
    {
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    public void SetOrbit(Vector2 center, float radius)
    {
        _centerPos = center;
        _radius = radius;
        _isOrbitSet = true;
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
                Vector2 origin = _isOrbitSet ? _centerPos : (Vector2)transform.position;
                Vector2 dir = (Vector2)target.position - origin;

                if (dir.sqrMagnitude > 0.001f)
                {
                    _lastFacingDir = dir.normalized;
                    
                    float angle = Mathf.Atan2(_lastFacingDir.y, _lastFacingDir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);

                    if (_isOrbitSet)
                    {
                        transform.position = _centerPos + (_lastFacingDir * _radius);
                    }
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        onTrackEnd?.Invoke();
    }

    public void SelfDestroy(float delay = 0f) => Destroy(gameObject, delay);
}