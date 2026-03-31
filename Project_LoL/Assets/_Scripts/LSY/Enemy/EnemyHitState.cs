using UnityEngine;

public class EnemyHitState : EnemyStateBase
{
    private float _timer;
    private float _animDuration;

    public EnemyHitState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _fsm.rigid.linearVelocity = Vector2.zero;
        
        if (_fsm.animator != null)
        {
            _fsm.animator.SetTrigger("3_Damaged");
            _animDuration = GetAnimationClipLength("Damage") > 0.1f ? GetAnimationClipLength("Damage") : 0.3f;
        }
        else
        {
            _animDuration = 0.3f;
        }
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        
        if (_timer < _animDuration) return;

        if (_fsm.isPlayerInAttackRange)
            _fsm.ChangeState(EnemyStateType.Attack);
        else if (_fsm.isPlayerInDetectRange)
            _fsm.ChangeState(EnemyStateType.Chase);
        else
            _fsm.ChangeState(EnemyStateType.Idle);
    }

    private float GetAnimationClipLength(string keyword)
    {
        if (_fsm.animator == null || _fsm.animator.runtimeAnimatorController == null) return 0.3f;

        foreach (AnimationClip clip in _fsm.animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToUpper().Contains(keyword.ToUpper()) || clip.name.ToUpper().Contains("HIT"))
            {
                return clip.length;
            }
        }
        return 0.3f;
    }
}