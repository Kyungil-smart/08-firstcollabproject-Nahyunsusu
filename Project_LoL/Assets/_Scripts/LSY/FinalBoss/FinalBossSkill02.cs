using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalBossSkill02 : FinalBossSkillBase
{
    [Header("프리팹 (Inspector 연결)")]
    public GameObject projectilePrefab;
    public GameObject aimIndicatorPrefab;
    public GameObject impactPrefab;

    private const float _trackDuration = 3f;
    private const float _stopDuration  = 2f;

    protected override int GetCurrentDamage()
    {
        return base.GetCurrentDamage(); 
    }

    private int GetProjectileCount()
    {
        return _boss.isPhase50
            ? Random.Range(6, 9)   
            : Random.Range(4, 7);  
    }

    protected override void OnExecute()
    {
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        int              count      = GetProjectileCount();
        List<GameObject> indicators = new List<GameObject>();
        Vector2          bossPos    = _boss.transform.position;

        for (int i = 0; i < count; i++)
        {
            if (aimIndicatorPrefab != null)
            {
                GameObject aimObj = Instantiate(aimIndicatorPrefab, bossPos, Quaternion.identity);
                FinalBossAimIndicator indicator = aimObj.GetComponent<FinalBossAimIndicator>();
                indicator?.Initialize(skillData.monsterSkillProjectileScaleX,
                                      skillData.monsterSkillProjectileScaleY,
                                      skillData.indicatorSprite);
                
                indicator?.StartTracking(_boss.playerTransform, _trackDuration, null);
                indicators.Add(aimObj);
            }
        }

        yield return new WaitForSeconds(_trackDuration);
        yield return new WaitForSeconds(_stopDuration);

        float currentRange = GetCurrentRange();

        foreach (GameObject aimObj in indicators)
        {
            if (aimObj == null) continue;

            FinalBossAimIndicator indicator = aimObj.GetComponent<FinalBossAimIndicator>();
            Vector2 dir = indicator != null ? indicator.lastFacingDir : Vector2.right;

            if (indicator != null) indicator.SelfDestroy();

            if (impactPrefab != null)
            {
                GameObject impactObj = Instantiate(impactPrefab, bossPos, Quaternion.identity);
                FinalBossImpact impact = impactObj.GetComponent<FinalBossImpact>();

                impact?.Initialize(GetCurrentDamage(),
                                   skillData.monsterSkillImpactScaleX,
                                   skillData.monsterSkillImpactScaleY,
                                   true,
                                   skillData.impactSprite);

                impact?.SetDirectional(dir, skillData.monsterSkillProjectileSpeed, currentRange);
            }
        }

        float travelTime = currentRange / Mathf.Max(1f, skillData.monsterSkillProjectileSpeed);
        yield return new WaitForSeconds(travelTime);

        FinishSkill();
    }
}