using UnityEngine;
using System.Collections;

public class FinalBossSkill05 : FinalBossSkillBase
{
    [Header("프리팹 (Inspector 연결)")]
    public GameObject projectilePrefab;
    public GameObject impactPrefab;

    private const float _chaseDuration    = 2f;   
    private const float _delayBeforeShoot = 0.5f; 
    private const float _respawnDelay     = 0.2f; 
    private const float _finalImpactDelay = 0.1f; 

    protected override void OnExecute()
    {
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        Transform player    = _boss.playerTransform;
        Vector2   bossPos   = _boss.transform.position;
        float     speed     = skillData.monsterSkillProjectileSpeed; 
        int       damage    = skillData.monsterSkillDamage;          

        Vector2 spawnPos    = player != null ? (Vector2)player.position : bossPos;
        GameObject projObj1 = SpawnProjectile(spawnPos);

        bool proj1Stopped = false;
        FinalBossProjectile proj1 = projObj1?.GetComponent<FinalBossProjectile>();
        proj1?.StartChasing(player, speed, _chaseDuration, () => proj1Stopped = true);

        yield return new WaitUntil(() => proj1Stopped || projObj1 == null);

        yield return new WaitForSeconds(_delayBeforeShoot);

        Vector2 proj1Pos = projObj1 != null ? (Vector2)projObj1.transform.position : spawnPos;

        GameObject impObj = SpawnImpact(bossPos, damage, skillData.monsterSkillImpactScaleX, skillData.monsterSkillImpactScaleY);
        FinalBossImpact impact = impObj?.GetComponent<FinalBossImpact>();

        bool impactReached = false;
        impact?.SetMoveToPosition(proj1Pos, speed, () => impactReached = true);

        yield return new WaitUntil(() => impactReached || impObj == null);

        impact?.StopMovement();
        if (projObj1 != null) Destroy(projObj1);

        yield return new WaitForSeconds(_respawnDelay);

        Vector2 newSpawnPos = player != null ? (Vector2)player.position : proj1Pos;
        GameObject projObj2 = SpawnProjectile(newSpawnPos);
        FinalBossProjectile proj2 = projObj2?.GetComponent<FinalBossProjectile>();

        bool proj2Stopped = false;
        proj2?.StartChasing(player, speed, _chaseDuration, () => proj2Stopped = true);

        yield return new WaitUntil(() => proj2Stopped || projObj2 == null);

        yield return new WaitForSeconds(_delayBeforeShoot);

        Vector2 proj2Pos = projObj2 != null ? (Vector2)projObj2.transform.position : newSpawnPos;

        bool impactReached2 = false;
        impact?.SetMoveToPosition(proj2Pos, speed, () => impactReached2 = true);

        yield return new WaitUntil(() => impactReached2 || impObj == null);

        if (projObj2 != null) Destroy(projObj2);
        if (impObj   != null) Destroy(impObj, _finalImpactDelay);

        yield return new WaitForSeconds(_finalImpactDelay);

        FinishSkill();
    }

    private GameObject SpawnProjectile(Vector2 pos)
    {
        if (projectilePrefab == null) return null;
        GameObject projObj = Instantiate(projectilePrefab, pos, Quaternion.identity);
        FinalBossProjectile proj = projObj.GetComponent<FinalBossProjectile>();
        proj?.Initialize(skillData.monsterSkillProjectileScaleX, skillData.monsterSkillProjectileScaleY, skillData.projectileSprite);
        return projObj;
    }

    private GameObject SpawnImpact(Vector2 pos, int damage, float scaleX, float scaleY)
    {
        if (impactPrefab == null) return null;
        GameObject impObj = Instantiate(impactPrefab, pos, Quaternion.identity);
        FinalBossImpact impact = impObj.GetComponent<FinalBossImpact>();
        impact?.Initialize(damage, scaleX, scaleY, true, skillData.impactSprite);
        return impObj;
    }
}