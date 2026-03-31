using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class MonsterProjectile : MonoBehaviour
{
    private MonsterSkillDataSO _skill;
    private Vector2 _direction;
    private Vector2 _startPosition;
    private int _damage;
    private bool _isInitialized = false;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb != null)
        {
            _rb.gravityScale = 0;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    public void Init(MonsterSkillDataSO skill, Vector2 start, Vector2 dir, int dmg)
    {
        _skill = skill;
        _direction = dir.normalized;
        _startPosition = start;
        _damage = dmg;

        transform.localScale = new Vector3(skill.damageRangeX, skill.damageRangeY, 1f);
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        _isInitialized = true;
    }

    private void FixedUpdate()
    {
        if (!_isInitialized || _skill == null) return;

        Vector2 nextPos = (Vector2)transform.position + _direction * (_skill.projectileSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(nextPos);

        if (Vector2.Distance(_startPosition, transform.position) >= _skill.range)
        {
            SkillPool.Instance.Despawn(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isInitialized || _skill == null) return;

        if (other.CompareTag("Enemy") || other.CompareTag("Boss")) return;

        if (other.TryGetComponent(out Damageable target))
        {
            target.TakeDamage(_damage);
            
            if (_skill.hitVfxPrefab != null)
            {
                GameObject vfx = SkillPool.Instance.Spawn(_skill.hitVfxPrefab, transform.position, Quaternion.identity);
                SkillPool.Instance.Despawn(vfx, 1.0f);
            }
            
            SkillPool.Instance.Despawn(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            SkillPool.Instance.Despawn(gameObject);
        }
    }
}