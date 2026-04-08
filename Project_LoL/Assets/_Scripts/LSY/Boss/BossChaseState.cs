using UnityEngine;

public class BossChaseState : BossStateBase
{
    public BossChaseState(BossFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _fsm.animator?.SetBool("1_Move", true);
    }

    public override void Update()
    {
        if (_fsm.isPlayerInAttackRange)
        {
            _fsm.ChangeState(BossStateType.Warning);
            return;
        }
        if (!_fsm.isPlayerInDetectRange)
        {
            _fsm.ChangeState(BossStateType.Idle);
            return;
        }

        MoveToPlayer();
        _fsm.FlipToPlayer();
    }

    public override void Exit()
    {
        _fsm.rigid.linearVelocity = Vector2.zero;
        _fsm.animator?.SetBool("1_Move", false);
    }

    private void MoveToPlayer()
    {
        if (_fsm.playerTransform == null) return;

        Vector2 dir = ((Vector2)_fsm.playerTransform.position
                       - (Vector2)_fsm.transform.position).normalized;
        _fsm.rigid.linearVelocity = dir * _fsm.data.moveSpeed;
    }
}