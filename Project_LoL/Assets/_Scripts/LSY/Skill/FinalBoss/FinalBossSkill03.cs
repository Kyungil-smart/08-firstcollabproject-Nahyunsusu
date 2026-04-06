using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalBossSkill03 : FinalBossSkillBase
{
    [Header("프리팹")]
    public GameObject projectilePrefab;
    [Tooltip("임팩트 (FinalBossRotatingImpact가 붙어있어야 함)")]
    public GameObject impactPrefab;
    public GameObject destroyEffectPrefab; 

    [Header("스킬 데미지 설정")]
    public int damage = 10;

    [Header("투사체 설정")]
    public float[] defaultOffsets = { 2f, -2f };
    public float[] phase50Offsets = { 4f, 2f, -2f, -4f };
    public float projectileScaleX = 1f;
    public float projectileScaleY = 1f;
    public float projectileTime = 2f;

    [Header("임팩트 설정")]
    public float impactScaleX = 1f;
    public float impactScaleY = 1f;
    public float impactTime = 2f;
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
        Vector2 bossPos = _boss.transform.position;
        float[] offsets = _boss.isPhase50 ? phase50Offsets : defaultOffsets;
        int projCount = offsets.Length;
        GameObject[] projObjects = new GameObject[projCount];

        for (int i = 0; i < projCount; i++)
        {
            Vector2 spawnPos = new Vector2(bossPos.x + offsets[i], bossPos.y);
            if (projectilePrefab != null)
            {
                GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                projObj.GetComponent<FinalBossProjectile>()?.Initialize(projectileScaleX, projectileScaleY);
                projObjects[i] = projObj;
                Destroy(projObj, projectileTime); 
            }
        }

        yield return new WaitForSeconds(_spawnDelay);

        for (int i = 0; i < projCount; i++)
        {
            if (projObjects[i] != null) SpawnImpact((Vector2)projObjects[i].transform.position);
        }

        yield return new WaitForSeconds(_impactInterval);

        for (int i = 0; i < projCount; i++)
        {
            if (projObjects[i] != null) SpawnImpact((Vector2)projObjects[i].transform.position);
        }

        yield return new WaitForSeconds(impactTime);
        FinishSkill();
    }

    private void SpawnImpact(Vector2 spawnPos)
    {
        if (impactPrefab == null || _boss.playerTransform == null) return;

        GameObject impactObj = Instantiate(impactPrefab, spawnPos, Quaternion.identity);
        FinalBossRotatingImpact impact = impactObj.GetComponent<FinalBossRotatingImpact>();
        
        if (impact != null)
        {
            impact.Initialize(GetCurrentDamage(), impactScaleX, impactScaleY, false);
            impact.SetStopOnHit();
            impact.onDestroyEffect = SpawnDestroyEffect;

            impact.onPlayerHit = (pos) => {
                SpawnImpact2(pos);
                Destroy(impact.gameObject);
            };

            float chaseDuration = impactTime - _directionalDuration;
            impact.SetDirectionalThenChase(Vector2.down, impactSpeed, _directionalDuration, _boss.playerTransform, chaseDuration);
        }
    }

    private void SpawnImpact2(Vector2 pos)
    {
        if (impactPrefab == null) return;
        GameObject impactObj = Instantiate(impactPrefab, pos, Quaternion.identity);
        impactObj.GetComponent<FinalBossImpact>()?.Initialize(GetCurrentDamage(), impactScaleX, impactScaleY, false);
        impactObj.GetComponent<FinalBossImpact>()?.SetStatic(impactTime);
    }

    private void SpawnDestroyEffect(Vector2 pos)
    {
        if (destroyEffectPrefab == null) return;
        Destroy(Instantiate(destroyEffectPrefab, pos, Quaternion.identity), _effectDuration);
    }
}