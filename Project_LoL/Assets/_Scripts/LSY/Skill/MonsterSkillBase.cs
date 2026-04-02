using UnityEngine;

public abstract class MonsterSkillBase : MonoBehaviour
{
    protected Rigidbody2D _rb;
    protected Animator    _animator;

    protected virtual void Awake()
    {
        _rb       = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    public abstract void Execute(MonsterSkillDataSO skillData, Vector2 targetPos, int damage);

    public virtual void StopSkill()
    {
        StopAllCoroutines(); 
    }
}