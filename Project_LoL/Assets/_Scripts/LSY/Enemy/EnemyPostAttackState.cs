using UnityEngine;

public class EnemyPostAttackState : EnemyStateBase
{
    private float _timer;
    private float _calculatedCooldown;

    public EnemyPostAttackState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter() 
    { 
        _timer = 0f; 
        
        float speed = _fsm.data.attackSpeed > 0f ? _fsm.data.attackSpeed : 1f;
        
        _calculatedCooldown = 1f / speed; 
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer < _calculatedCooldown) return;

        _fsm.FlipToPlayer();

        if (_fsm.isPlayerInAttackRange)
            _fsm.ChangeState(EnemyStateType.Attack);
        else if (_fsm.isPlayerInDetectRange)
            _fsm.ChangeState(EnemyStateType.Chase);
        else
            _fsm.ChangeState(EnemyStateType.Idle);
    }
}