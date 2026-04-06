using UnityEngine;
using System.Collections;

public class FinalBossSkill05 : FinalBossSkillBase
{
    [Header("프리팹")]
    public GameObject projectilePrefab;
    [Tooltip("임팩트 (FinalBossRotatingImpact가 붙어있어야 함)")]
    public GameObject impactPrefab;

    [Header("스킬 데미지 설정")]
    public int damage = 10;

    [Header("크기 설정")]
    public float projectileScaleX = 1f;
    public float projectileScaleY = 1f;
    public float impactScaleX = 1f;
    public float impactScaleY = 1f;

    [Header("이동 및 타이밍 설정")]
    public float moveSpeed = 12f;
    public float chaseDuration = 2f;
    public float delayBeforeShoot = 0.5f;
    public float respawnDelay = 0.2f;
    public float finalImpactDelay = 0.1f;

    protected override int GetCurrentDamage()
    {
        return damage + (_boss != null ? _boss.baseAttack : 0); 
    }

    protected override void OnExecute()
    {
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        Transform player = _boss.playerTransform;
        Vector2 bossPos = _boss.transform.position;
        int currentDamage = GetCurrentDamage();

        Vector2 spawnPos = player != null ? (Vector2)player.position : bossPos;
        GameObject projObj1 = SpawnProjectile(spawnPos);
        bool proj1Stopped = false;
        projObj1?.GetComponent<FinalBossProjectile>()?.StartChasing(player, moveSpeed, chaseDuration, () => proj1Stopped = true);

        yield return new WaitUntil(() => proj1Stopped || projObj1 == null);
        yield return new WaitForSeconds(delayBeforeShoot);

        Vector2 proj1Pos = projObj1 != null ? (Vector2)projObj1.transform.position : spawnPos;
        GameObject impObj1 = SpawnImpact(bossPos, currentDamage, impactScaleX, impactScaleY);
        bool impactReached1 = false;
        impObj1?.GetComponent<FinalBossRotatingImpact>()?.SetMoveToPosition(proj1Pos, moveSpeed, () => impactReached1 = true);

        yield return new WaitUntil(() => impactReached1 || impObj1 == null);
        if (projObj1 != null) Destroy(projObj1);

        yield return new WaitForSeconds(respawnDelay);

        Vector2 newSpawnPos = player != null ? (Vector2)player.position : proj1Pos;
        GameObject projObj2 = SpawnProjectile(newSpawnPos);
        bool proj2Stopped = false;
        projObj2?.GetComponent<FinalBossProjectile>()?.StartChasing(player, moveSpeed, chaseDuration, () => proj2Stopped = true);

        yield return new WaitUntil(() => proj2Stopped || projObj2 == null);
        yield return new WaitForSeconds(delayBeforeShoot);

        Vector2 proj2Pos = projObj2 != null ? (Vector2)projObj2.transform.position : newSpawnPos;
        if (impObj1 != null) Destroy(impObj1);

        GameObject impObj2 = SpawnImpact(proj1Pos, currentDamage, impactScaleX, impactScaleY);
        bool impactReached2 = false;
        impObj2?.GetComponent<FinalBossRotatingImpact>()?.SetMoveToPosition(proj2Pos, moveSpeed, () => impactReached2 = true);

        yield return new WaitUntil(() => impactReached2 || impObj2 == null);
        if (projObj2 != null) Destroy(projObj2);
        if (impObj2 != null) Destroy(impObj2, finalImpactDelay);

        yield return new WaitForSeconds(finalImpactDelay);
        FinishSkill();
    }

    private GameObject SpawnProjectile(Vector2 pos)
    {
        if (projectilePrefab == null) return null;
        GameObject obj = Instantiate(projectilePrefab, pos, Quaternion.identity);
        obj.GetComponent<FinalBossProjectile>()?.Initialize(projectileScaleX, projectileScaleY);
        return obj;
    }

    private GameObject SpawnImpact(Vector2 pos, int damageValue, float scaleX, float scaleY)
    {
        if (impactPrefab == null) return null;
        GameObject obj = Instantiate(impactPrefab, pos, Quaternion.identity);
        obj.GetComponent<FinalBossRotatingImpact>()?.Initialize(damageValue, scaleX, scaleY, true);
        return obj;
    }
}