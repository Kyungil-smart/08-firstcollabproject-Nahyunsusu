using UnityEngine;

public class MonsterSkillHitbox : MonoBehaviour
{
    private MonsterSkillDataSO _skill;
    private int _damage;
    private bool _hasHit = false;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
        }
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Init(MonsterSkillDataSO skill, int damage)
    {
        _skill = skill;
        _damage = damage;
        _hasHit = false;

        transform.localScale = new Vector3(skill.damageRangeX, skill.damageRangeY, 1f);

        SkillPool.Instance.Despawn(gameObject, 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit || _skill == null) return;

        if (other.CompareTag("Enemy") || other.CompareTag("Boss")) return;

        if (((1 << other.gameObject.layer) & _skill.targetLayer.value) != 0)
        {
            if (other.TryGetComponent(out Damageable target))
            {
                target.TakeDamage(_damage);
                
                if (_skill.hitVfxPrefab != null)
                {
                    GameObject vfx = SkillPool.Instance.Spawn(_skill.hitVfxPrefab, other.transform.position, Quaternion.identity);
                    SkillPool.Instance.Despawn(vfx, 1.0f);
                }

                _hasHit = true;

                SkillPool.Instance.Despawn(gameObject);
            }
        }
    }
}