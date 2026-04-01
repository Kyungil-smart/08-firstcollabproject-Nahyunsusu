using UnityEngine;
using System.Collections;

public class FinalBossSkill04 : FinalBossSkillBase
{
    [Header("프리팹 (Inspector 연결)")]
    public GameObject aimIndicatorPrefab; 
    public GameObject impactPrefab;       

    private const float _trackDuration = 2f;   
    private const float _stopDuration  = 0.5f; 

    protected override int GetCurrentDamage()
    {
        int damage = base.GetCurrentDamage(); 
        if (_boss.isPhase30) damage += 15;         
        if (_boss.isPhase20) damage += 20;         
        if (_boss.isPhase10) damage += 25;         
        return damage;
    }

    protected override void OnExecute()
    {
        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        Vector2 bossPos = _boss.transform.position;

        FinalBossAimIndicator indicator = null;
        if (aimIndicatorPrefab != null)
        {
            GameObject aimObj = Instantiate(aimIndicatorPrefab, bossPos, Quaternion.identity);
            indicator = aimObj.GetComponent<FinalBossAimIndicator>();
            indicator?.Initialize(skillData.monsterSkillProjectileScaleX, 
                                  skillData.monsterSkillProjectileScaleY, 
                                  skillData.indicatorSprite);
        }

        bool trackDone = false;
        Vector2 finalDir = Vector2.right;

        if (indicator != null)
        {
            indicator.StartTracking(_boss.playerTransform, _trackDuration, () =>
            {
                if (indicator != null) finalDir = indicator.lastFacingDir;
                trackDone = true;
            });
        }
        else
        {
            if (_boss.playerTransform != null)
                finalDir = ((Vector2)_boss.playerTransform.position - bossPos).normalized;
            trackDone = true;
        }

        yield return new WaitUntil(() => trackDone || indicator == null);

        yield return new WaitForSeconds(_stopDuration);

        if (indicator != null) indicator.SelfDestroy();

        float currentRange = GetCurrentRange();

        if (impactPrefab != null)
        {
            GameObject      impactObj = Instantiate(impactPrefab, bossPos, Quaternion.identity);
            FinalBossImpact impact    = impactObj.GetComponent<FinalBossImpact>();
            
            impact?.Initialize(GetCurrentDamage(),
                               skillData.monsterSkillImpactScaleX,  
                               skillData.monsterSkillImpactScaleY,  
                               true,
                               skillData.impactSprite);
            
            impact?.SetDirectional(finalDir,
                                   skillData.monsterSkillProjectileSpeed, 
                                   currentRange,                          
                                   skillData.monsterSkillImpactTime);     
        }

        float travelTime = currentRange / Mathf.Max(1f, skillData.monsterSkillProjectileSpeed);
        yield return new WaitForSeconds(travelTime);

        FinishSkill();
    }
}