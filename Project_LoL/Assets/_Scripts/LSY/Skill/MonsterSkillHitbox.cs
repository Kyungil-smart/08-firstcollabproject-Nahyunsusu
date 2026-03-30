using UnityEngine;

public class MonsterSkillHitbox : MonoBehaviour
{
    private int _damage;
    private LayerMask _targetLayer;
    private Vector2 _size;

    public void Init(int damage, Vector2 size, float duration, LayerMask targetLayer)
    {
        _damage = damage;
        _size = size;
        _targetLayer = targetLayer;

        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, _size, transform.eulerAngles.z, _targetLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Damageable d)) d.TakeDamage(_damage);
        }

        Destroy(gameObject, duration);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, _size);
    }
}