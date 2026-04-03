using UnityEngine;
using System.Collections;

public class FinalBossSkill05 : FinalBossSkillBase
{
    [Header("프리팹")]
    [Tooltip("투사체")]
    public GameObject projectilePrefab;
    [Tooltip("임팩트")]
    public GameObject impactPrefab;

    [Header("스킬 데미지 설정")]
    public int damage = 10;

    [Header("크기 설정")]
    [Tooltip("투사체 크기 X")]
    public float projectileScaleX = 1f;
    [Tooltip("투사체 크기 Y")]
    public float projectileScaleY = 1f;
    [Tooltip("임팩트 크기 X")]
    public float impactScaleX = 1f;
    [Tooltip("임팩트 크기 Y")]
    public float impactScaleY = 1f;

    [Header("이동 및 타이밍 설정")]
    [Tooltip("투사체 및 임팩트 속도")]
    public float moveSpeed = 12f;
    [Tooltip("투사체가 플레이어를 추적하는 시간")]
    public float chaseDuration = 2f;
    [Tooltip("추적 완료 후 실제 공격 발사 전 대기 시간")]
    public float delayBeforeShoot = 0.5f;
    [Tooltip("1타 공격 후 2타 추적 시작 전 대기 시간")]
    public float respawnDelay = 0.2f;
    [Tooltip("2타 타격 후 스킬 종료 전 대기 시간")]
    public float finalImpactDelay = 0.1f;

    protected override int GetCurrentDamage()
    {
        int bossAttack = _boss != null ? _boss.baseAttack : 0;
        return damage + bossAttack; 
    }

    protected override void OnExecute()
    {
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        Transform player  = _boss.playerTransform;
        Vector2   bossPos = _boss.transform.position;
        int       currentDamage  = GetCurrentDamage();

        Vector2 spawnPos    = player != null ? (Vector2)player.position : bossPos;
        GameObject projObj1 = SpawnProjectile(spawnPos);

        bool proj1Stopped = false;
        FinalBossProjectile proj1 = projObj1?.GetComponent<FinalBossProjectile>();
        proj1?.StartChasing(player, moveSpeed, chaseDuration, () => proj1Stopped = true);

        yield return new WaitUntil(() => proj1Stopped || projObj1 == null);
        yield return new WaitForSeconds(delayBeforeShoot);

        Vector2 proj1Pos = projObj1 != null ? (Vector2)projObj1.transform.position : spawnPos;

        GameObject impObj1 = SpawnImpact(bossPos, currentDamage, impactScaleX, impactScaleY);
        FinalBossImpact impact1 = impObj1?.GetComponent<FinalBossImpact>();

        bool impactReached1 = false;
        impact1?.SetMoveToPosition(proj1Pos, moveSpeed, () => impactReached1 = true);

        yield return new WaitUntil(() => impactReached1 || impObj1 == null);

        impact1?.StopMovement();
        if (projObj1 != null) Destroy(projObj1);

        yield return new WaitForSeconds(respawnDelay);

        Vector2 newSpawnPos = player != null ? (Vector2)player.position : proj1Pos;
        GameObject projObj2 = SpawnProjectile(newSpawnPos);
        FinalBossProjectile proj2 = projObj2?.GetComponent<FinalBossProjectile>();

        bool proj2Stopped = false;
        proj2?.StartChasing(player, moveSpeed, chaseDuration, () => proj2Stopped = true);

        yield return new WaitUntil(() => proj2Stopped || projObj2 == null);
        yield return new WaitForSeconds(delayBeforeShoot);

        Vector2 proj2Pos = projObj2 != null ? (Vector2)projObj2.transform.position : newSpawnPos;

        if (impObj1 != null) Destroy(impObj1);

        GameObject impObj2 = SpawnImpact(proj1Pos, currentDamage, impactScaleX, impactScaleY);
        FinalBossImpact impact2 = impObj2?.GetComponent<FinalBossImpact>();

        bool impactReached2 = false;
        impact2?.SetMoveToPosition(proj2Pos, moveSpeed, () => impactReached2 = true);

        yield return new WaitUntil(() => impactReached2 || impObj2 == null);

        if (projObj2 != null) Destroy(projObj2);
        if (impObj2  != null) Destroy(impObj2, finalImpactDelay);

        yield return new WaitForSeconds(finalImpactDelay);

        FinishSkill();
    }

    private GameObject SpawnProjectile(Vector2 pos)
    {
        if (projectilePrefab == null) return null;
        GameObject projObj = Instantiate(projectilePrefab, pos, Quaternion.identity);
        FinalBossProjectile proj = projObj.GetComponent<FinalBossProjectile>();
        
        proj?.Initialize(projectileScaleX, projectileScaleY);
        return projObj;
    }

    private GameObject SpawnImpact(Vector2 pos, int damageValue, float scaleX, float scaleY)
    {
        if (impactPrefab == null) return null;
        GameObject impObj = Instantiate(impactPrefab, pos, Quaternion.identity);
        FinalBossImpact impact = impObj.GetComponent<FinalBossImpact>();
        
        impact?.Initialize(damageValue, scaleX, scaleY, true);
        return impObj;
    }
}