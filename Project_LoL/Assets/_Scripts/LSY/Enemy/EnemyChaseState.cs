using UnityEngine;

public class EnemyChaseState : EnemyStateBase
{
    public EnemyChaseState(EnemyFSM fsm) : base(fsm) { }

    public override void Update()
    {
        if (_fsm.isPlayerInAttackRange)
        {
            _fsm.ChangeState(EnemyStateType.Attack);
            return;
        }
        if (!_fsm.isPlayerInDetectRange)
        {
            _fsm.ChangeState(EnemyStateType.Idle);
            return;
        }

        Vector2 dir = ((Vector2)_fsm.playerTransform.position
                       - (Vector2)_fsm.transform.position).normalized;
        _fsm.rigid.linearVelocity = dir * _fsm.data.moveSpeed;
    }

    public override void Exit()
    {
        _fsm.rigid.linearVelocity = Vector2.zero;
    }
}