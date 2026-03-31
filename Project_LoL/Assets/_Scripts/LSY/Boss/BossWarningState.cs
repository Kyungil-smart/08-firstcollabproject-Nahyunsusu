using UnityEngine;

public class BossWarningState : BossStateBase
{
    public BossWarningState(BossFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _fsm.SelectRandomSkill();
        
        if (_fsm.selectedSkill == null || _fsm.playerTransform == null)
        {
            _fsm.ChangeState(BossStateType.Chase);
            return;
        }

        _fsm.initialTargetPos = _fsm.playerTransform.position;
        
        _fsm.FlipToPlayer();

        _fsm.ChangeState(BossStateType.Attack);
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}