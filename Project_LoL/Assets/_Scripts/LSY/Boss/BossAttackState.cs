using UnityEngine;

public class BossAttackState : BossStateBase
{
    private float _timer;

    public BossAttackState(BossFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _fsm.rigid.linearVelocity = Vector2.zero;
        if (_fsm.warningSignObj != null) _fsm.warningSignObj.SetActive(false);

        _fsm.animator?.SetTrigger("2_Attack");
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _fsm.data.attackDuration)
        {
            _fsm.ChangeState(BossStateType.Chase);
        }
    }
}