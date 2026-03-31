using UnityEngine;

public class BossHitState : BossStateBase
{
    private float _timer;

    public BossHitState(BossFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _fsm.rigid.linearVelocity = Vector2.zero;
        _fsm.animator?.SetTrigger("3_Damaged");
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _fsm.data.hitDuration) return;

        if (_fsm.isPlayerInAttackRange)
            _fsm.ChangeState(BossStateType.Warning);
        else if (_fsm.isPlayerInDetectRange)
            _fsm.ChangeState(BossStateType.Chase);
        else
            _fsm.ChangeState(BossStateType.Idle);
    }
}
