using UnityEngine;

public class EnemyHitState : EnemyStateBase
{
    private float _timer;

    public EnemyHitState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _fsm.rigid.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _fsm.data.hitDuration) return;

        if (_fsm.isPlayerInAttackRange)
            _fsm.ChangeState(EnemyStateType.Attack);
        else if (_fsm.isPlayerInDetectRange)
            _fsm.ChangeState(EnemyStateType.Chase);
        else
            _fsm.ChangeState(EnemyStateType.Idle);
    }

    public override void Exit() { }
}