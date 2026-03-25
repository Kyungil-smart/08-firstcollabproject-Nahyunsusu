public class EnemyIdleState : EnemyStateBase
{
    public EnemyIdleState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _fsm.rigid.linearVelocity = UnityEngine.Vector2.zero;
        _fsm.animator?.SetBool("1_Move", false);
    }

    public override void Update()
    {
        if (_fsm.isPlayerInDetectRange)
            _fsm.ChangeState(EnemyStateType.Chase);
    }
}