using System.Collections;
using UnityEngine;

public class Boss3Skill10 : BossSkillBase
{
    private GameObject _warningObj;
    private Animator _bossAnimator;

    private void Awake()
    {
        _bossAnimator = GetComponentInChildren<Animator>();
    }

    public override void ExecuteSkill(MonsterSkillDataSO skill, Vector2 targetPos, int baseDamage)
    {
        if (skill == null) return;
        StartCoroutine(AreaBombChargingRoutine(skill, targetPos, skill.baseDamage + baseDamage));
    }

    private IEnumerator AreaBombChargingRoutine(MonsterSkillDataSO skill, Vector2 targetPos, int finalDamage)
    {
        if (skill.warningPrefab != null && skill.warningDuration > 0)
        {
            _warningObj = SkillPool.Instance.Spawn(skill.warningPrefab, targetPos, Quaternion.identity);
            
            Animator warnAnim = _warningObj.GetComponentInChildren<Animator>();
            if (warnAnim != null) warnAnim.enabled = false;

            float timer = 0f;
            float duration = skill.warningDuration;

            while (timer < duration)
            {
                if (_warningObj == null || !_warningObj.activeInHierarchy) yield break; 
                timer += Time.deltaTime;
                float progress = Mathf.Clamp01(timer / duration);
                
                float curX = skill.damageRangeX * progress;
                float curY = skill.damageRangeY * progress;
                _warningObj.transform.localScale = new Vector3(curX, curY, 1f);
                yield return null;
            }
            if (_warningObj != null) { SkillPool.Instance.Despawn(_warningObj); _warningObj = null; }
        }
        else { yield return new WaitForSeconds(skill.warningDuration); }

        if (_bossAnimator != null) _bossAnimator.SetTrigger("2_Attack");

        if (skill.skillPrefab != null)
        {
            GameObject explosion = SkillPool.Instance.Spawn(skill.skillPrefab, targetPos, Quaternion.identity);
            explosion.transform.localScale = new Vector3(skill.damageRangeX, skill.damageRangeY, 1f);
            
            float animTime = GetAnimationClipLength(explosion);
            SkillPool.Instance.Despawn(explosion, animTime); 
        }

        Physics2D.SyncTransforms();
        Collider2D[] hits = Physics2D.OverlapBoxAll(targetPos, new Vector2(skill.damageRangeX, skill.damageRangeY), 0f, skill.targetLayer);
        
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy") && !hit.CompareTag("Boss") && hit.TryGetComponent(out Damageable target))
            {
                target.TakeDamage(finalDamage);
                if (skill.hitVfxPrefab != null) 
                {
                    GameObject vfx = SkillPool.Instance.Spawn(skill.hitVfxPrefab, hit.transform.position, Quaternion.identity);
                    SkillPool.Instance.Despawn(vfx, 1.0f);
                }
            }
        }
    }

    private float GetAnimationClipLength(GameObject obj)
    {
        Animator anim = obj.GetComponentInChildren<Animator>();
        if (anim == null) return 1.0f;
        RuntimeAnimatorController rac = anim.runtimeAnimatorController;
        if (rac == null || rac.animationClips.Length == 0) return 1.0f;
        return rac.animationClips[0].length;
    }

    public override void StopSkill()
    {
        base.StopSkill();
        if (_warningObj != null) { SkillPool.Instance.Despawn(_warningObj); _warningObj = null; }
    }
}