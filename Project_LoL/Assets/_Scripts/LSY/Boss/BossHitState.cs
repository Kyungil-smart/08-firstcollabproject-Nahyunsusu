using UnityEngine;

public class BossHitState : BossStateBase
{
    private float _timer;
    private float _animDuration;

    public BossHitState(BossFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _fsm.rigid.linearVelocity = Vector2.zero;
        
        if (_fsm.animator != null)
        {
            _fsm.animator.SetTrigger("3_Damaged");
            float length = GetAnimationClipLength("Damage");
            _animDuration = length > 0.1f ? length : 0.3f;
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
            _fsm.ChangeState(BossStateType.Warning);
        else if (_fsm.isPlayerInDetectRange)
            _fsm.ChangeState(BossStateType.Chase);
        else
            _fsm.ChangeState(BossStateType.Idle);
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