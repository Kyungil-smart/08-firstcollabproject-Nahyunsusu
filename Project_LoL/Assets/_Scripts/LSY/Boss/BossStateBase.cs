public class BossStateBase
{
    protected BossFSM _fsm;

    public BossStateBase(BossFSM fsm) { _fsm = fsm; }

    public virtual void Enter()  { }
    public virtual void Update() { }
    public virtual void Exit()   { }
}
