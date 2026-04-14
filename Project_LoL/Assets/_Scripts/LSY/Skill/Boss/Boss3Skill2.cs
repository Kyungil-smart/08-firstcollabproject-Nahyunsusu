using System.Collections;
using UnityEngine;

public class Boss3Skill2 : BossSkillBase
{
    private GameObject _warningObj;
    private Animator _bossAnimator;

    private void Awake() => _bossAnimator = GetComponentInChildren<Animator>();

    public override void ExecuteSkill(MonsterSkillDataSO skill, Vector2 targetPos, int baseDamage)
    {
        if (skill == null) return;
        int finalDamage = skill.baseDamage + baseDamage;
        StartCoroutine(AreaBombChargingRoutine(skill, targetPos, finalDamage));
    }

    private IEnumerator AreaBombChargingRoutine(MonsterSkillDataSO skill, Vector2 targetPos, int finalDamage)
    {
        if (skill.warningPrefab != null && skill.warningDuration > 0)
        {
            _warningObj = SkillPool.Instance.Spawn(skill.warningPrefab, targetPos, Quaternion.identity);
            
            float timer = 0f;
            while (timer < skill.warningDuration)
            {
                if (_warningObj == null || !_warningObj.activeInHierarchy) yield break; 
                timer += Time.deltaTime;
                float progress = Mathf.Clamp01(timer / skill.warningDuration);
                
                _warningObj.transform.localScale = new Vector3(skill.damageRangeX * progress, skill.damageRangeY * progress, 1f);
                yield return null;
            }
            if (_warningObj != null) { SkillPool.Instance.Despawn(_warningObj); _warningObj = null; }
        }
        else { yield return new WaitForSeconds(skill.warningDuration); }

        if (_bossAnimator != null) _bossAnimator.SetTrigger("2_Attack");

        if (skill.skillPrefab != null)
        {
            GameObject explosion = SkillPool.Instance.Spawn(skill.skillPrefab, targetPos, Quaternion.identity);
            
            if (explosion.TryGetComponent(out MonsterSkillHitbox hitbox))
            {
                hitbox.Init(skill, finalDamage);
            }
            else
            {
                explosion.transform.localScale = new Vector3(skill.damageRangeX, skill.damageRangeY, 1f);
                float animTime = skill.skillDuration > 0 ? skill.skillDuration : 1.0f;
                SkillPool.Instance.Despawn(explosion, animTime); 
            }
        }
    }

    public override void StopSkill()
    {
        base.StopSkill();
        if (_warningObj != null) { SkillPool.Instance.Despawn(_warningObj); _warningObj = null; }
    }
}