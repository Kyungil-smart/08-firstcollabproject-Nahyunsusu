using UnityEngine;

public class EnemyMeleeSkill : MonoBehaviour
{
    public void ExecuteMelee(MonsterSkillDataSO skill, Vector2 targetPos, int baseDamage)
    {
        if (skill == null || skill.skillPrefab == null) return;

        Vector2 originPos = (Vector2)transform.position;
        Vector2 dir = (targetPos - originPos).normalized;
        if (dir == Vector2.zero) dir = transform.right;

        float spawnOffset = 0.5f; 
        Vector2 spawnPos = originPos + (dir * spawnOffset);

        GameObject hitboxObj = SkillPool.Instance.Spawn(skill.skillPrefab, spawnPos, Quaternion.identity);
        hitboxObj.transform.right = dir;
        hitboxObj.transform.localScale = new Vector3(skill.damageRangeX, skill.damageRangeY, 1f);

        if (hitboxObj.TryGetComponent(out BoxCollider2D boxCol))
        {
            boxCol.size = new Vector2(1f, 1f); 
            boxCol.offset = new Vector2(0.5f, 0); 
            boxCol.isTrigger = true;
        }

        Physics2D.SyncTransforms();
        Collider2D[] results = Physics2D.OverlapBoxAll(hitboxObj.transform.position + (Vector3)dir * (skill.damageRangeX / 2f), 
            new Vector2(skill.damageRangeX, skill.damageRangeY), 
            hitboxObj.transform.eulerAngles.z, skill.targetLayer);

        foreach (var hit in results)
        {
            if (hit.CompareTag("Player") && hit.TryGetComponent(out PlayerController target))
            {
                target.TakeDamage(skill.baseDamage + baseDamage);
            }
        }
        SkillPool.Instance.Despawn(hitboxObj, 0.1f);
    }
}