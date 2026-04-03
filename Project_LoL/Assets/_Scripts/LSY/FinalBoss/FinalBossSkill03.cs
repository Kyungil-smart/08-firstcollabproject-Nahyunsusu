using UnityEngine;
using System.Collections;

public class FinalBossSkill03 : FinalBossSkillBase
{
    [Header("프리팹")]
    [Tooltip("투사체")]
    public GameObject projectilePrefab;
    [Tooltip("임팩트")]
    public GameObject impactPrefab;
    [Tooltip("파괴 이펙트")]
    public GameObject destroyEffectPrefab; 

    [Header("스킬 데미지 설정")]
    public int damage = 10;

    [Header("투사체 설정")]
    [Tooltip("기본 스폰 위치")]
    public float[] defaultOffsets = { 2f, -2f };
    [Tooltip("체력 50% 이하일 때 스폰 위치")]
    public float[] phase50Offsets = { 4f, 2f, -2f, -4f };
    
    [Tooltip("투사체 크기 X")]
    public float projectileScaleX = 1f;
    [Tooltip("투사체 크기 Y")]
    public float projectileScaleY = 1f;
    [Tooltip("투사체 존재 시간")]
    public float projectileTime = 2f;

    [Header("임팩트 설정")]
    [Tooltip("임팩트 크기 X")]
    public float impactScaleX = 1f;
    [Tooltip("임팩트 크기 Y")]
    public float impactScaleY = 1f;
    [Tooltip("임팩트 존재 시간")]
    public float impactTime = 2f;
    [Tooltip("임팩트 속도")]
    public float impactSpeed = 10f;

    private const float _spawnDelay          = 1f;    
    private const float _impactInterval      = 0.1f;  
    private const float _directionalDuration = 0.3f;  
    private const float _effectDuration      = 0.5f;  

    protected override int GetCurrentDamage()
    {
        int bossAttack = _boss != null ? _boss.baseAttack : 0;
        int finalDamage = damage + bossAttack; 
        
        if (_boss != null && _boss.isPhase20) finalDamage += 20;
        
        return finalDamage;
    }

    protected override void OnExecute()
    {
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        Vector2 bossPos   = _boss.transform.position;
        bool    isPhase50 = _boss != null && _boss.isPhase50;

        float[] offsets     = isPhase50 ? phase50Offsets : defaultOffsets;
        int     projCount   = offsets.Length;
        GameObject[] projObjects = new GameObject[projCount];

        for (int i = 0; i < projCount; i++)
        {
            Vector2 spawnPos = new Vector2(bossPos.x + offsets[i], bossPos.y);

            if (projectilePrefab != null)
            {
                GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                FinalBossProjectile proj = projObj.GetComponent<FinalBossProjectile>();
                
                proj?.Initialize(projectileScaleX, projectileScaleY);
                projObjects[i] = projObj;
                Destroy(projObj, projectileTime); 
            }
        }

        yield return new WaitForSeconds(_spawnDelay);

        for (int i = 0; i < projCount; i++)
        {
            if (projObjects[i] != null)
                SpawnImpact((Vector2)projObjects[i].transform.position);
        }

        yield return new WaitForSeconds(_impactInterval);

        for (int i = 0; i < projCount; i++)
        {
            if (projObjects[i] != null)
                SpawnImpact((Vector2)projObjects[i].transform.position);
        }

        yield return new WaitForSeconds(impactTime);
        FinishSkill();
    }

    private void SpawnImpact(Vector2 spawnPos)
    {
        if (impactPrefab == null || _boss == null || _boss.playerTransform == null) return;

        GameObject      impactObj = Instantiate(impactPrefab, spawnPos, Quaternion.identity);
        FinalBossImpact impact    = impactObj.GetComponent<FinalBossImpact>();
        if (impact == null) return;

        impact.Initialize(GetCurrentDamage(),
                          impactScaleX,
                          impactScaleY,
                          false);

        impact.SetStopOnHit();
        impact.onDestroyEffect = SpawnDestroyEffect;

        FinalBossImpact capturedImpact = impact;
        impact.onPlayerHit = (pos) =>
        {
            SpawnImpact2(pos);
            if (capturedImpact != null) Destroy(capturedImpact.gameObject);
        };

        float chaseDuration = impactTime - _directionalDuration;
        impact.SetDirectionalThenChase(Vector2.down,
                                       impactSpeed,
                                       _directionalDuration,
                                       _boss.playerTransform,
                                       chaseDuration);
    }

    private void SpawnImpact2(Vector2 pos)
    {
        if (impactPrefab == null) return;

        GameObject      impactObj = Instantiate(impactPrefab, pos, Quaternion.identity);
        FinalBossImpact impact2   = impactObj.GetComponent<FinalBossImpact>();
        if (impact2 == null) return;

        impact2.Initialize(GetCurrentDamage(),
                           impactScaleX,
                           impactScaleY,
                           false);

        impact2.onDestroyEffect = SpawnDestroyEffect;
        impact2.SetStatic(impactTime);
    }

    private void SpawnDestroyEffect(Vector2 pos)
    {
        if (destroyEffectPrefab == null) return;
        GameObject effect = Instantiate(destroyEffectPrefab, pos, Quaternion.identity);
        Destroy(effect, _effectDuration);
    }
}