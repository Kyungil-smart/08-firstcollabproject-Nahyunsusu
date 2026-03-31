using UnityEngine;

public class BossAttackState : BossStateBase
{
    private float _timer;
    private float _totalDuration;

    public BossAttackState(BossFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _fsm.rigid.linearVelocity = Vector2.zero;

        if (_fsm.selectedSkill != null)
        {
            float skillActionTime = _fsm.data.attackDuration;

            if (_fsm.selectedSkill.skillType == MonsterSkillType.Dash)
            {
                float dashTime = _fsm.selectedSkill.range / Mathf.Max(0.1f, _fsm.selectedSkill.projectileSpeed);
                skillActionTime = Mathf.Max(skillActionTime, dashTime);
            }

            _totalDuration = _fsm.selectedSkill.warningDuration + skillActionTime + 0.5f;
        }
        else
        {
            _totalDuration = _fsm.data.attackDuration;
        }

        _fsm.TriggerAttackSkill();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _totalDuration)
        {
            _fsm.ChangeState(BossStateType.PostAttack);
        }
    }

    public override void Exit()
    {
    }
}