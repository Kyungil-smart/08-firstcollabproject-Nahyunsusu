public abstract class EnemyStateBase
{
    protected EnemyFSM _fsm;

    public EnemyStateBase(EnemyFSM fsm) { _fsm = fsm; }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}