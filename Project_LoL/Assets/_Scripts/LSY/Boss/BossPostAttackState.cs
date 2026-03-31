using UnityEngine;

public class BossPostAttackState : BossStateBase
{
    private float _timer;

    public BossPostAttackState(BossFSM fsm) : base(fsm) { }

    public override void Enter() { _timer = 0f; }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < _fsm.data.attackCooldown) return;

        _fsm.FlipToPlayer();

        if (_fsm.isPlayerInAttackRange)
            _fsm.ChangeState(BossStateType.Warning);
        else if (_fsm.isPlayerInDetectRange)
            _fsm.ChangeState(BossStateType.Chase);
        else
            _fsm.ChangeState(BossStateType.Idle);
    }
}
