using System.Collections;
using UnityEngine;

public class Boss2Skill : BossSkillBase
{
    private GameObject _warningObj;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public override void ExecuteSkill(MonsterSkillDataSO skill, Vector2 targetPos, int baseDamage)
    {
        if (skill == null) return;
        StartCoroutine(DoubleSlashRoutine(skill, targetPos, skill.baseDamage + baseDamage));
    }

    private IEnumerator DoubleSlashRoutine(MonsterSkillDataSO skill, Vector2 targetPos, int damage)
    {
        Vector2 startPos = (Vector2)transform.position;
        Vector2 dir = (targetPos - startPos).normalized;
        if (dir == Vector2.zero) dir = transform.right;
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        if (skill.warningPrefab != null && skill.warningDuration > 0)
        {
            _warningObj = SkillPool.Instance.Spawn(skill.warningPrefab, startPos, rot);
            float timer = 0f;
            while (timer < skill.warningDuration)
            {
                if (_warningObj == null || !_warningObj.activeInHierarchy) yield break;
                timer += Time.deltaTime;
                float progress = Mathf.Clamp01(timer / skill.warningDuration);
                float currentLength = skill.damageRangeX * progress;

                _warningObj.transform.localScale = new Vector3(currentLength, skill.damageRangeY, 1f);
                _warningObj.transform.position = startPos + (dir * (currentLength * 0.5f));
                yield return null;
            }
            if (_warningObj != null) { SkillPool.Instance.Despawn(_warningObj); _warningObj = null; }
        }
        else { yield return new WaitForSeconds(skill.warningDuration); }

        if (_animator != null) _animator.SetTrigger("2_Attack");

        Vector2 attackPos = startPos + dir * (skill.damageRangeX / 2f);
        float effectTime = skill.skillDuration > 0 ? skill.skillDuration : 0.2f;
        
        if (skill.skillPrefab != null)
        {
            GameObject obj1 = SkillPool.Instance.Spawn(skill.skillPrefab, attackPos, rot);
            obj1.transform.right = dir;
            ApplyHit(obj1, skill, damage, angle);
            
            SkillPool.Instance.Despawn(obj1, effectTime);
        }
        
        yield return new WaitForSeconds(skill.attackInterval > 0 ? skill.attackInterval : 0.3f);
        
        GameObject prefab2 = skill.skillPrefab2 != null ? skill.skillPrefab2 : skill.skillPrefab;
        if (prefab2 != null)
        {
            GameObject obj2 = SkillPool.Instance.Spawn(prefab2, attackPos, rot);
            obj2.transform.right = dir;
            ApplyHit(obj2, skill, damage, angle);
            
            SkillPool.Instance.Despawn(obj2, effectTime);
        }
    }

    private void ApplyHit(GameObject obj, MonsterSkillDataSO skill, int damage, float angle)
    {
        if (obj != null) obj.transform.localScale = new Vector3(skill.damageRangeX, skill.damageRangeY, 1f);
        Physics2D.SyncTransforms();
        
        Vector2 hitPos = obj != null ? (Vector2)obj.transform.position : (Vector2)transform.position;
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitPos, new Vector2(skill.damageRangeX, skill.damageRangeY), angle, skill.targetLayer);
        
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy") && !hit.CompareTag("Boss") && hit.TryGetComponent(out Damageable target))
            {
                target.TakeDamage(damage);
                if (skill.hitVfxPrefab != null) 
                {
                    GameObject vfx = SkillPool.Instance.Spawn(skill.hitVfxPrefab, hit.transform.position, Quaternion.identity);
                    SkillPool.Instance.Despawn(vfx, 1.0f);
                }
            }
        }
    }

    public override void StopSkill()
    {
        base.StopSkill();
        if (_warningObj != null) { SkillPool.Instance.Despawn(_warningObj); _warningObj = null; }
    }
}