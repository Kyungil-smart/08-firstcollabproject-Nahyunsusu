public class EnemyIdleState : EnemyStateBase
{
    public EnemyIdleState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _fsm.rigid.linearVelocity = UnityEngine.Vector2.zero;
    }

    public override void Update()
    {
        if (_fsm.isPlayerInDetectRange)
            _fsm.ChangeState(EnemyStateType.Chase);
    }

    public override void Exit() { }
}