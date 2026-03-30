using UnityEngine;

public class MonsterSkillExecutor : MonoBehaviour
{
    [Header("기본 히트박스 프리팹")]
    public MonsterSkillHitbox meleeHitboxPrefab;

    public void TryExecute(MonsterSkillDataSO skill, Transform origin, Vector2 facingDir, int baseAttack)
    {
        if (skill == null) return;
        int finalDamage = skill.damage + baseAttack;

        if (skill.skillType == MonsterSkillType.Ranged)
        {
            if (skill.projectilePrefab != null)
            {
                MonsterProjectile proj = Instantiate(skill.projectilePrefab);
                proj.Init(facingDir, origin.position, finalDamage, skill.projectileSpeed, skill.range, skill.damageRangeX, skill.damageRangeY);
            }
        }
        else
        {
            Vector2 spawnPos = (Vector2)origin.position + (facingDir * (skill.range * 0.5f));
            if (skill.skillType == MonsterSkillType.Cross) spawnPos = origin.position;

            MonsterSkillHitbox hitbox = Instantiate(meleeHitboxPrefab, spawnPos, Quaternion.identity);
            
            float angle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;
            hitbox.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            hitbox.Init(finalDamage, new Vector2(skill.damageRangeX, skill.damageRangeY), 0.2f, skill.targetLayer);
        }
    }
}