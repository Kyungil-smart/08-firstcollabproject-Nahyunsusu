using UnityEngine;
using System;

public abstract class FinalBossSkillBase : MonoBehaviour
{
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
        return 0; 
    }

    protected virtual float GetCurrentRange()
    {
        return 0f;
    }
}