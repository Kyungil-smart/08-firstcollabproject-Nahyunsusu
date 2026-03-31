using UnityEngine;

public abstract class MonsterSkillBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator animator;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    public abstract void Execute(MonsterSkillDataSO skillData, Vector2 targetPos, int damage);

    public virtual void StopSkill()
    {
        StopAllCoroutines(); 
    }
}