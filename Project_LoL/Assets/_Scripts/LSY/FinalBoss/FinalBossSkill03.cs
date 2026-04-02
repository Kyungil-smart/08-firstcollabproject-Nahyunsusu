using UnityEngine;
using System.Collections;

public class FinalBossSkill03 : FinalBossSkillBase
{
    [Header("프리팹 (Inspector 연결)")]
    public GameObject projectilePrefab;
    public GameObject impactPrefab;
    public GameObject destroyEffectPrefab; 

    private const float _spawnDelay          = 1f;    
    private const float _impactInterval      = 0.1f;  
    private const float _directionalDuration = 0.3f;  
    private const float _effectDuration      = 0.5f;  

    protected override int GetCurrentDamage()
    {
        int damage = base.GetCurrentDamage(); 
        if (_boss.isPhase20) damage += 20;
        return damage;
    }

    protected override void OnExecute()
    {
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        Vector2 bossPos   = _boss.transform.position;
        bool    isPhase50 = _boss.isPhase50;

        float[] offsets     = isPhase50
            ? new float[] { 4f, 2f, -2f, -4f }
            : new float[] { 2f, -2f };
        int     projCount   = offsets.Length;
        GameObject[] projObjects = new GameObject[projCount];

        for (int i = 0; i < projCount; i++)
        {
            Vector2 spawnPos = new Vector2(bossPos.x + offsets[i], bossPos.y);

            if (projectilePrefab != null)
            {
                GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                FinalBossProjectile proj = projObj.GetComponent<FinalBossProjectile>();
                proj?.Initialize(skillData.monsterSkillProjectileScaleX,
                                 skillData.monsterSkillProjectileScaleY,
                                 skillData.projectileSprite);
                projObjects[i] = projObj;
                Destroy(projObj, skillData.monsterSkillProjectileTime); 
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

        yield return new WaitForSeconds(skillData.monsterSkillImpactTime);
        FinishSkill();
    }

    private void SpawnImpact(Vector2 spawnPos)
    {
        if (impactPrefab == null || _boss.playerTransform == null) return;

        GameObject      impactObj = Instantiate(impactPrefab, spawnPos, Quaternion.identity);
        FinalBossImpact impact    = impactObj.GetComponent<FinalBossImpact>();
        if (impact == null) return;

        impact.Initialize(GetCurrentDamage(),
                          skillData.monsterSkillImpactScaleX,
                          skillData.monsterSkillImpactScaleY,
                          false,
                          skillData.impactSprite);

        impact.SetStopOnHit();
        impact.onDestroyEffect = SpawnDestroyEffect;

        FinalBossImpact capturedImpact = impact;
        impact.onPlayerHit = (pos) =>
        {
            SpawnImpact2(pos);
            Destroy(capturedImpact.gameObject);
        };

        float chaseDuration = skillData.monsterSkillImpactTime - _directionalDuration;
        impact.SetDirectionalThenChase(Vector2.down,
                                       skillData.monsterSkillProjectileSpeed,
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
                           skillData.monsterSkillImpactScaleX,
                           skillData.monsterSkillImpactScaleY,
                           false,
                           skillData.impactSprite);

        impact2.onDestroyEffect = SpawnDestroyEffect;
        impact2.SetStatic(skillData.monsterSkillImpactTime);
    }

    private void SpawnDestroyEffect(Vector2 pos)
    {
        if (destroyEffectPrefab == null) return;
        GameObject effect = Instantiate(destroyEffectPrefab, pos, Quaternion.identity);
        Destroy(effect, _effectDuration);
    }
}