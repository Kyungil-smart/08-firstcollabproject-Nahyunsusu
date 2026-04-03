using UnityEngine;
using System.Collections;

public class FinalBossSkill04 : FinalBossSkillBase
{
    [Header("프리팹")]
    [Tooltip("조준")]
    public GameObject aimIndicatorPrefab;
    [Tooltip("임팩트")]
    public GameObject impactPrefab;

    [Header("스킬 데미지 설정")]
    public int damage = 10;

    [Header("조준 설정")]
    [Tooltip("조준 크기 X")]
    public float indicatorScaleX = 1f;
    [Tooltip("조준 크기 Y")]
    public float indicatorScaleY = 1f;
    [Tooltip("조준 시간")]
    public float trackDuration = 2f;
    [Tooltip("정지 시간")]
    public float stopDuration  = 0.5f;

    [Header("임팩트 설정")]
    [Tooltip("임팩트 크기 X")]
    public float impactScaleX = 1f;
    [Tooltip("임팩트 크기 Y")]
    public float impactScaleY = 1f;
    [Tooltip("임팩트 속도")]
    public float impactSpeed = 20f;
    [Tooltip("임팩트 사거리")]
    public float impactRange = 25f;
    [Tooltip("임팩트 존재 시간")]
    public float impactTime = 2f;

    protected override int GetCurrentDamage()
    {
        int bossAttack = _boss != null ? _boss.baseAttack : 0;
        int finalDamage = damage + bossAttack;
        
        if (_boss != null)
        {
            if (_boss.isPhase10) finalDamage += 25;
            else if (_boss.isPhase20) finalDamage += 20;
            else if (_boss.isPhase30) finalDamage += 15;
        }      
        return finalDamage;
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
            
            indicator?.Initialize(indicatorScaleX, indicatorScaleY);
        }

        bool trackDone = false;
        Vector2 finalDir = Vector2.right;

        if (indicator != null)
        {
            indicator.StartTracking(_boss.playerTransform, trackDuration, () =>
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

        yield return new WaitForSeconds(stopDuration);

        if (indicator != null) indicator.SelfDestroy();

        if (impactPrefab != null)
        {
            GameObject      impactObj = Instantiate(impactPrefab, bossPos, Quaternion.identity);
            FinalBossImpact impact    = impactObj.GetComponent<FinalBossImpact>();
            
            impact?.Initialize(GetCurrentDamage(),
                               impactScaleX,  
                               impactScaleY,  
                               true);
            
            impact?.SetDirectional(finalDir,
                                   impactSpeed, 
                                   impactRange,                          
                                   impactTime);
        }

        float travelTime = impactRange / Mathf.Max(1f, impactSpeed);
        yield return new WaitForSeconds(travelTime);

        FinishSkill();
    }
}