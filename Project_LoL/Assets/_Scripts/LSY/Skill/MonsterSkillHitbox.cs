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

        float duration = skill.skillDuration > 0 ? skill.skillDuration : 0.2f;
        SkillPool.Instance.Despawn(gameObject, duration);
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
                    
                    float scale = _skill.impactScale > 0 ? _skill.impactScale : 1f;
                    vfx.transform.localScale = new Vector3(scale, scale, 1f);

                    float impactDuration = _skill.impactTime > 0 ? _skill.impactTime : 1f;
                    SkillPool.Instance.Despawn(vfx, impactDuration);
                }

                _hasHit = true;
                
                SkillPool.Instance.Despawn(gameObject);
            }
        }
    }
}