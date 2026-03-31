using UnityEngine;

public abstract class BossSkillBase : MonoBehaviour
{
    public abstract void ExecuteSkill(MonsterSkillDataSO skill, Vector2 targetPos, int baseDamage);
    
    public virtual void StopSkill() 
    {
        StopAllCoroutines();
    }
}