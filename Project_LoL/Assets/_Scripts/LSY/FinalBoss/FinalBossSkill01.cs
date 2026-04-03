using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalBossSkill01 : FinalBossSkillBase
{
    [Header("프리팹")]
    [Tooltip("투사체")]
    public GameObject projectilePrefab;
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
    [Tooltip("투사체 존재 시간")]
    public float projectileTime = 1f;

    [Header("임팩트 설정")]
    [Tooltip("임팩트 크기 X")]
    public float impactScaleX = 1f;
    [Tooltip("임팩트 크기 Y")]
    public float impactScaleY = 1f;
    [Tooltip("임팩트 존재 시간")]
    public float impactTime = 1f;

    protected override int GetCurrentDamage()
    {
        int bossAttack = _boss != null ? _boss.baseAttack : 0;
        int finalDamage = damage + bossAttack; 
        
        if (_boss != null && _boss.isPhase20) finalDamage += 30;
        
        return finalDamage;
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
        int                count          = GetProjectileCount();
        List<Vector2>      spawnPositions = new List<Vector2>();
        List<GameObject>   projObjects    = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = _boss.GetRandomRoomPosition();
            spawnPositions.Add(pos);

            if (projectilePrefab != null)
            {
                GameObject projObj = Instantiate(projectilePrefab, pos, Quaternion.identity);
                FinalBossProjectile proj = projObj.GetComponent<FinalBossProjectile>();
                proj?.Initialize(projectileScaleX, projectileScaleY);
                projObjects.Add(projObj);
            }
        }

        yield return new WaitForSeconds(projectileTime);

        foreach (GameObject obj in projObjects)
        {
            if (obj != null) Destroy(obj);
        }

        foreach (Vector2 pos in spawnPositions)
        {
            if (impactPrefab == null) continue;

            GameObject    impactObj = Instantiate(impactPrefab, pos, Quaternion.identity);
            FinalBossImpact impact  = impactObj.GetComponent<FinalBossImpact>();
            impact?.Initialize(GetCurrentDamage(),
                               impactScaleX,
                               impactScaleY,
                               false);
            impact?.SetStatic(impactTime);
        }

        yield return new WaitForSeconds(impactTime);
        FinishSkill();
    }
}