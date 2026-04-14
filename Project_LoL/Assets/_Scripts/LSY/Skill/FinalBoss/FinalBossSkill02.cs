using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalBossSkill02 : FinalBossSkillBase
{
    [Header("프리팹")]
    [Tooltip("투사체")]
    public GameObject projectilePrefab;
    [Tooltip("조준")]
    public GameObject aimIndicatorPrefab;
    [Tooltip("임팩트")]
    public GameObject impactPrefab;

    [Header("스킬 데미지 설정")]
    public int damage = 10;

    [Header("투사체 설정")]
    [Tooltip("기본 투사체 갯수 (최소~최대)")]
    public int minProjectileCount = 4;
    public int maxProjectileCount = 7;
    [Tooltip("보스 체력 50% 이하일 때 투사체 갯수 (최소~최대)")]
    public int phase50MinProjectileCount = 6;
    public int phase50MaxProjectileCount = 9;

    [Tooltip("투사체 크기 X")]
    public float projectileScaleX = 1f;
    [Tooltip("투사체 크기 Y")]
    public float projectileScaleY = 1f;
    
    [Tooltip("조준 시간")]
    public float trackDuration = 3f;
    [Tooltip("정지 시간")]
    public float stopDuration = 2f;

    [Header("임팩트 설정")]
    [Tooltip("임팩트 크기 X")]
    public float impactScaleX = 1f;
    [Tooltip("임팩트 크기 Y")]
    public float impactScaleY = 1f;
    [Tooltip("임팩트 사거리")]
    public float impactRange = 20f;
    [Tooltip("임팩트 속도")]
    public float impactSpeed = 15f;

    protected override int GetCurrentDamage()
    {
        int bossAttack = _boss != null ? _boss.baseAttack : 0;
        return damage + bossAttack;
    }

    private int GetProjectileCount()
    {
        if (_boss != null && _boss.isPhase50)
            return Random.Range(phase50MinProjectileCount, phase50MaxProjectileCount);
        else
            return Random.Range(minProjectileCount, maxProjectileCount);
    }

    protected override void OnExecute()
    {
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        int              count       = GetProjectileCount();
        List<GameObject> projObjects = new List<GameObject>();
        List<GameObject> indicators  = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = _boss.GetRandomRoomPosition();

            if (projectilePrefab != null)
            {
                GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                FinalBossProjectile proj = projObj.GetComponent<FinalBossProjectile>();
                proj?.Initialize(projectileScaleX, projectileScaleY);
                projObjects.Add(projObj);
            }

            if (aimIndicatorPrefab != null)
            {
                GameObject aimObj = Instantiate(aimIndicatorPrefab, spawnPos, Quaternion.identity);
                FinalBossAimIndicator indicator = aimObj.GetComponent<FinalBossAimIndicator>();
                
                indicator?.Initialize(impactRange, projectileScaleY);
                
                float radius = projectileScaleX * 0.5f;
                indicator?.SetOrbit(spawnPos, radius);
                
                indicator?.StartTracking(_boss.playerTransform, trackDuration, null);
                indicators.Add(aimObj);
            }
        }

        yield return new WaitForSeconds(trackDuration);
        yield return new WaitForSeconds(stopDuration);

        foreach (GameObject projObj in projObjects)
        {
            if (projObj != null) Destroy(projObj);
        }

        foreach (GameObject aimObj in indicators)
        {
            if (aimObj == null) continue;

            FinalBossAimIndicator indicator = aimObj.GetComponent<FinalBossAimIndicator>();
            
            Vector2 dir     = indicator != null ? indicator.lastFacingDir : Vector2.right;
            Vector2 firePos = aimObj.transform.position;

            if (indicator != null) indicator.SelfDestroy();

            if (impactPrefab != null)
            {
                GameObject impactObj = Instantiate(impactPrefab, firePos, Quaternion.identity);
                FinalBossImpact impact = impactObj.GetComponent<FinalBossImpact>();

                impact?.Initialize(GetCurrentDamage(), impactScaleX, impactScaleY, true);
                impact?.SetDirectional(dir, impactSpeed, impactRange);
            }
        }

        float travelTime = impactRange / Mathf.Max(1f, impactSpeed);
        yield return new WaitForSeconds(travelTime);

        FinishSkill();
    }
}