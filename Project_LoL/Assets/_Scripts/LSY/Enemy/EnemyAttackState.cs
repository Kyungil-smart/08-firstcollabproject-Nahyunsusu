using UnityEngine;

public class EnemyAttackState : EnemyStateBase
{
    private float _timer;
    private float _animDuration;

    public EnemyAttackState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _fsm.rigid.linearVelocity = Vector2.zero;

        if (_fsm.playerTransform != null)
            _fsm.initialTargetPos = _fsm.playerTransform.position;

        _fsm.FlipToPlayer();

        if (_fsm.currentSkill != null)
        {
            if (_fsm.currentSkill.skillType == MonsterSkillType.Melee && _fsm.meleeSkill != null)
                _fsm.meleeSkill.ExecuteMelee(_fsm.currentSkill, _fsm.initialTargetPos, _fsm.data.attackDamage);
            else if (_fsm.currentSkill.skillType == MonsterSkillType.Ranged && _fsm.rangedSkill != null)
                _fsm.rangedSkill.ExecuteRanged(_fsm.currentSkill, _fsm.initialTargetPos, _fsm.data.attackDamage);
        }

        if (_fsm.animator != null) 
        {
            _fsm.animator.SetTrigger("2_Attack");
            _animDuration = GetAnimationClipLength("Attack");
        }
        else
        {
            _animDuration = 1.0f;
        }
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _animDuration)
        {
            _fsm.ChangeState(EnemyStateType.PostAttack);
        }
    }

    private float GetAnimationClipLength(string keyword)
    {
        if (_fsm.animator == null || _fsm.animator.runtimeAnimatorController == null) return 1.0f;

        foreach (AnimationClip clip in _fsm.animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToUpper().Contains(keyword.ToUpper()))
            {
                return clip.length;
            }
        }
        return 1.0f;
    }

    public override void Exit() { }
}