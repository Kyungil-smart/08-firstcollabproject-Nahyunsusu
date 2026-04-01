using System.Collections;
using UnityEngine;

public class Boss3Skill4 : BossSkillBase
{
    private GameObject _warningObj;
    private Animator _bossAnimator;

    private void Awake() => _bossAnimator = GetComponentInChildren<Animator>();

    public override void ExecuteSkill(MonsterSkillDataSO skill, Vector2 targetPos, int baseDamage)
    {
        if (skill == null) return;
        StartCoroutine(LaserRoutine(skill, targetPos, skill.baseDamage + baseDamage));
    }

    private IEnumerator LaserRoutine(MonsterSkillDataSO skill, Vector2 targetPos, int damage)
    {
        Vector2 startPos = (Vector2)transform.position;
        Vector2 dir = (targetPos - startPos).normalized;
        if (dir == Vector2.zero) dir = transform.right;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        
        float maxLen = skill.damageRangeX > 0 ? skill.damageRangeX : 20f; 
        float width = skill.damageRangeY > 0 ? skill.damageRangeY : 1f; 

        if (skill.warningPrefab != null && skill.warningDuration > 0)
        {
            _warningObj = SkillPool.Instance.Spawn(skill.warningPrefab, startPos, rot);
            float timer = 0f;
            while (timer < skill.warningDuration)
            {
                if (_warningObj == null || !_warningObj.activeInHierarchy) yield break; 
                timer += Time.deltaTime;
                float progress = Mathf.Clamp01(timer / skill.warningDuration);
                float currentLength = maxLen * progress; 
                _warningObj.transform.localScale = new Vector3(currentLength, width, 1f);
                _warningObj.transform.position = startPos + (dir * (currentLength * 0.5f));
                yield return null;
            }
            if (_warningObj != null) SkillPool.Instance.Despawn(_warningObj);
        }
        else { yield return new WaitForSeconds(skill.warningDuration); }

        if (_bossAnimator != null) _bossAnimator.SetTrigger("2_Attack");

        Vector2 finalCenterPos = startPos + dir * (maxLen * 0.5f);

        for (int i = 0; i < skill.attackCount; i++)
        {
            if (skill.mainVfxPrefab != null)
            {
                GameObject laser = SkillPool.Instance.Spawn(skill.mainVfxPrefab, finalCenterPos, rot);
                laser.transform.right = dir; 
                laser.transform.localScale = new Vector3(maxLen, width, 1f);
                SkillPool.Instance.Despawn(laser, skill.skillDuration); 
            }

            Physics2D.SyncTransforms();
            Collider2D[] hits = Physics2D.OverlapBoxAll(finalCenterPos, new Vector2(maxLen, width), angle, skill.targetLayer);

            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Enemy") && !hit.CompareTag("Boss") && hit.TryGetComponent(out PlayerController target))
                {
                    target.TakeDamage(damage);
                    
                    if (skill.hitVfxPrefab != null) 
                    {
                        GameObject vfx = SkillPool.Instance.Spawn(skill.hitVfxPrefab, hit.transform.position, Quaternion.identity);
                        float scale = skill.impactScale > 0 ? skill.impactScale : 1f;
                        vfx.transform.localScale = new Vector3(scale, scale, 1f);
                        float duration = skill.impactTime > 0 ? skill.impactTime : 1f;
                        SkillPool.Instance.Despawn(vfx, duration);
                    }
                }
            }
            if (i < skill.attackCount - 1) yield return new WaitForSeconds(skill.attackInterval);
        }
    }

    public override void StopSkill()
    {
        base.StopSkill();
        if (_warningObj != null) SkillPool.Instance.Despawn(_warningObj);
    }
}