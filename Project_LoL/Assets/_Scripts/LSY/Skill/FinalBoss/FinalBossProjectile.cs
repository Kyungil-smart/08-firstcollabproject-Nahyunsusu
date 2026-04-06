using UnityEngine;
using System;
using System.Collections;

public class FinalBossProjectile : MonoBehaviour
{
    private Coroutine _chaseCoroutine;

    public Vector2 position => transform.position;

    public void Initialize(float scaleX, float scaleY)
    {
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    public void StartChasing(Transform target, float speed, float chaseDuration, Action onStopped)
    {
        if (_chaseCoroutine != null) StopCoroutine(_chaseCoroutine);
        _chaseCoroutine = StartCoroutine(ChaseRoutine(target, speed, chaseDuration, onStopped));
    }

    private IEnumerator ChaseRoutine(Transform target, float speed, float duration, Action onStopped)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (target != null)
            {
                Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
                transform.Translate(dir * speed * Time.deltaTime, Space.World);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        onStopped?.Invoke();
    }

    public void SelfDestroy(float delay = 0f)
    {
        Destroy(gameObject, delay);
    }
}