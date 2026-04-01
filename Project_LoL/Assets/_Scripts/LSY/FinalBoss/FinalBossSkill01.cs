using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalBossSkill01 : FinalBossSkillBase
{
    [Header("프리팹 (Inspector 연결)")]
    public GameObject projectilePrefab;
    public GameObject impactPrefab;

    protected override int GetCurrentDamage()
    {
        int damage = base.GetCurrentDamage(); 
        if (_boss.isPhase20) damage += 30;
        return damage;
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
                proj?.Initialize(skillData.monsterSkillProjectileScaleX,
                                 skillData.monsterSkillProjectileScaleY,
                                 skillData.projectileSprite);
                projObjects.Add(projObj);
            }
        }

        yield return new WaitForSeconds(skillData.monsterSkillProjectileTime);

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
                               skillData.monsterSkillImpactScaleX,
                               skillData.monsterSkillImpactScaleY,
                               false,
                               skillData.impactSprite);
            impact?.SetStatic(skillData.monsterSkillImpactTime);
        }

        yield return new WaitForSeconds(skillData.monsterSkillImpactTime);
        FinishSkill();
    }
}