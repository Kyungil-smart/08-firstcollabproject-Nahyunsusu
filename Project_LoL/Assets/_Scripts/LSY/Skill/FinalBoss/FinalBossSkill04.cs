using UnityEngine;
using System.Collections;

public class FinalBossSkill04 : FinalBossSkillBase
{
    [Header("프리팹 설정")]
    public GameObject aimIndicatorPrefab; 
    public GameObject impactPrefab;       

    [Header("레이저 범위 설정")]
    [Tooltip("레이저의 총 사거리 (X축 스케일)")]
    public float laserRange = 25f;       
    [Tooltip("레이저의 두께 (Y축 스케일)")]
    public float laserThickness = 2.0f;  
    [Tooltip("보스 중심에서 레이저가 시작될 거리")]
    public float spawnOffset = 1.5f;     

    [Header("스킬 데미지 설정")]
    public int damage = 10; 

    [Header("타이밍 설정")]
    public float trackDuration = 2.5f;
    public float stopDuration  = 0.5f;
    public float impactTime    = 1.0f;

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
        FinalBossLaserAimIndicator indicator = null;

        if (aimIndicatorPrefab != null)
        {
            GameObject aimObj = Instantiate(aimIndicatorPrefab, bossPos, Quaternion.identity);
            indicator = aimObj.GetComponent<FinalBossLaserAimIndicator>();
            
            indicator?.Initialize(laserRange, laserThickness, spawnOffset, _boss.transform);
        }

        bool trackDone = false;
        Vector2 finalDir = Vector2.right;

        if (indicator != null)
        {
            indicator.StartTracking(_boss.playerTransform, trackDuration, () => {
                finalDir = indicator.lastFacingDir;
                trackDone = true;
            });
        }
        else trackDone = true;

        yield return new WaitUntil(() => trackDone);
        yield return new WaitForSeconds(stopDuration);

        Vector2 firePos = bossPos + (finalDir * spawnOffset);
        if (indicator != null) indicator.SelfDestroy();

        if (impactPrefab != null)
        {
            float angle = Mathf.Atan2(finalDir.y, finalDir.x) * Mathf.Rad2Deg;
            
            Vector2 laserCenterPos = firePos + (finalDir * laserRange * 0.5f);
            
            GameObject impactObj = Instantiate(impactPrefab, laserCenterPos, Quaternion.Euler(0, 0, angle));
            FinalBossImpact impactScript = impactObj.GetComponent<FinalBossImpact>();
            
            impactScript?.Initialize(GetCurrentDamage(), laserRange, laserThickness, true);
            impactScript?.SetStatic(impactTime);
        }

        yield return new WaitForSeconds(impactTime);
        FinishSkill();
    }
}