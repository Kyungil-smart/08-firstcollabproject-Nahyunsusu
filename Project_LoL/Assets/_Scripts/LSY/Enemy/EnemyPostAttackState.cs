using UnityEngine;

public class EnemyPostAttackState : EnemyStateBase
{
    private float _timer;

    public EnemyPostAttackState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter() { _timer = 0f; }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < _fsm.data.attackCooldown) return;

        _fsm.FlipToPlayer();

        if (_fsm.isPlayerInAttackRange)
            _fsm.ChangeState(EnemyStateType.Attack);
        else if (_fsm.isPlayerInDetectRange)
            _fsm.ChangeState(EnemyStateType.Chase);
        else
            _fsm.ChangeState(EnemyStateType.Idle);
    }
}
