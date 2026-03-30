public class EnemyStateBase
{
    protected EnemyFSM _fsm;

    public EnemyStateBase(EnemyFSM fsm) { _fsm = fsm; }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}