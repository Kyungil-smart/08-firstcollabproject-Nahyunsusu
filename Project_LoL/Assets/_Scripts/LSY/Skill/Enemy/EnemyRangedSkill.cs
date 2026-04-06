using UnityEngine;

public class EnemyRangedSkill : MonoBehaviour
{
    public void ExecuteRanged(MonsterSkillDataSO skill, Vector2 targetPos, int baseDamage)
    {
        if (skill == null || skill.skillPrefab == null) return;

        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        if (dir == Vector2.zero) dir = transform.right;

        Vector2 spawnPos = (Vector2)transform.position + (dir * 0.5f);

        GameObject projectileObj = SkillPool.Instance.Spawn(skill.skillPrefab, spawnPos, Quaternion.identity);
        projectileObj.transform.right = dir;

        if (projectileObj.TryGetComponent(out MonsterProjectile proj))
        {
            proj.Init(skill, spawnPos, dir, skill.baseDamage + baseDamage);
        }
    }
}