using UnityEngine;

public class EnemyAttackState : EnemyStateBase
{
    private float _timer;

    public EnemyAttackState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _fsm.rigid.linearVelocity = Vector2.zero;

        if (_fsm.playerTransform != null)
            _fsm.initialTargetPos = _fsm.playerTransform.position;

        _fsm.FlipToPlayer();

        if (_fsm.currentSkill != null)
        {
            if (_fsm.currentSkill.skillType == MonsterSkillType.Melee && _fsm.meleeSkill != null)
            {
                _fsm.meleeSkill.ExecuteMelee(_fsm.currentSkill, _fsm.initialTargetPos, _fsm.data.attackDamage);
            }
            else if (_fsm.currentSkill.skillType == MonsterSkillType.Ranged && _fsm.rangedSkill != null)
            {
                _fsm.rangedSkill.ExecuteRanged(_fsm.currentSkill, _fsm.initialTargetPos, _fsm.data.attackDamage);
            }
        }

        if (_fsm.animator != null) _fsm.animator.SetTrigger("2_Attack");
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _fsm.data.attackDuration)
        {
            _fsm.ChangeState(EnemyStateType.PostAttack);
        }
    }

    public override void Exit() { }
}