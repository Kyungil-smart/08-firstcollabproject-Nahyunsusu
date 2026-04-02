using UnityEngine;
using System;
using System.Collections;

public class FinalBossImpact : MonoBehaviour
{
    private int  _damage;
    private bool _passThrough;
    private bool _isActive  = true;
    private bool _stopOnHit = false;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _col;

    public Action<Vector2> onDestroyEffect;
    public Action<Vector2> onPlayerHit;

    public Vector2 position => transform.position;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;
    }

    public void Initialize(int damage, float scaleX, float scaleY, bool passThrough = false, Sprite sprite = null)
    {
        _damage = damage;
        _passThrough = passThrough;
        transform.localScale = new Vector3(scaleX, scaleY, 1f);
        if (sprite != null) _spriteRenderer.sprite = sprite;
    }

    public void SetStatic(float lifetime)
    {
        if (lifetime > 0f) StartCoroutine(StaticRoutine(lifetime));
    }

    private IEnumerator StaticRoutine(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        if (_isActive) DestroyWithEffect();
    }

    public void SetDirectional(Vector2 direction, float speed, float range = 0f, float lifetime = 0f)
    {
        StartCoroutine(DirectionalRoutine(direction.normalized, speed, range, lifetime));
    }

    private IEnumerator DirectionalRoutine(Vector2 dir, float speed, float range, float lifetime)
    {
        float distanceTraveled = 0f;
        float elapsed = 0f;

        while (_isActive)
        {
            float step = speed * Time.deltaTime;
            transform.Translate(dir * step, Space.World);
            distanceTraveled += step;
            elapsed += Time.deltaTime;

            bool rangeOver = range > 0f && distanceTraveled >= range;
            bool timeOver = lifetime > 0f && elapsed >= lifetime;

            if (rangeOver || timeOver)
            {
                DestroyWithEffect();
                yield break;
            }

            yield return null;
        }
    }

    public void SetDirectionalThenChase(Vector2 startDir, float speed, float directionalDuration, Transform chaseTarget,
        float chaseDuration)
    {
        StartCoroutine(DirectionalThenChaseRoutine(startDir.normalized, speed, directionalDuration, chaseTarget,
            chaseDuration));
    }

    private IEnumerator DirectionalThenChaseRoutine(Vector2 dir, float speed, float directionalDuration,
        Transform chaseTarget, float chaseDuration)
    {
        float elapsed = 0f;
        while (_isActive && elapsed < directionalDuration)
        {
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (_isActive && elapsed < chaseDuration)
        {
            if (chaseTarget != null)
            {
                Vector2 chaseDir = ((Vector2)chaseTarget.position - (Vector2)transform.position).normalized;
                transform.Translate(chaseDir * speed * Time.deltaTime, Space.World);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (_isActive) DestroyWithEffect();
    }

    public void SetMoveToPosition(Vector2 targetPos, float speed, Action onReached)
    {
        StartCoroutine(MoveToPositionRoutine(targetPos, speed, onReached));
    }

    private IEnumerator MoveToPositionRoutine(Vector2 targetPos, float speed, Action onReached)
    {
        while (_isActive)
        {
            float step = speed * Time.deltaTime;
            float dist = Vector2.Distance(transform.position, targetPos);

            if (dist <= step)
            {
                transform.position = targetPos;
                onReached?.Invoke();
                yield break;
            }

            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
            transform.Translate(dir * step, Space.World);
            yield return null;
        }
    }

    public void StopMovement() => StopAllCoroutines();

    public void SetStopOnHit() => _stopOnHit = true;

    public void ForceDestroy()
    {
        _isActive = false;
        Destroy(gameObject);
    }

    private void DestroyWithEffect(float delay = 0f)
    {
        if (!_isActive) return;
        _isActive = false;

        Vector2 pos = transform.position;
        onDestroyEffect?.Invoke(pos);
        Destroy(gameObject, delay);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive) return;
        if (!other.CompareTag("Player")) return;

        Damageable damageable = other.GetComponent<Damageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(_damage);
        }

        if (_stopOnHit)
        {
            StopMovement();
            _col.enabled = false;
            onPlayerHit?.Invoke(transform.position);
        }
        else if (!_passThrough)
        {
            onPlayerHit?.Invoke(transform.position);
            DestroyWithEffect();
        }
    }
}