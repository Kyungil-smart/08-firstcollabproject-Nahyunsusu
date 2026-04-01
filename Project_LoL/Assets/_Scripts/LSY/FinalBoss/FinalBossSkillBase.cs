using UnityEngine;
using System;

public abstract class FinalBossSkillBase : MonoBehaviour
{
    [Header("스킬 데이터 (ScriptableObject 연결)")]
    public FinalBossSkillData skillData;

    protected FinalBossFSM _boss;
    private   Action       _onFinished;

    public void Execute(FinalBossFSM boss, Action onFinished)
    {
        _boss       = boss;
        _onFinished = onFinished;
        OnExecute();
    }

    protected abstract void OnExecute();

    protected void FinishSkill()
    {
        _onFinished?.Invoke();
    }

    protected virtual int GetCurrentDamage()
    {
        int baseDamage = skillData.monsterSkillDamage;
        int bossAttack = _boss != null ? _boss.baseAttack : 0;
        
        return baseDamage + bossAttack; 
    }

    protected virtual float GetCurrentRange()
    {
        return skillData.monsterSkillRange;
    }
}