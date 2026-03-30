using UnityEngine;

public class EnemyAttackState : EnemyStateBase
{
    private float _timer;
    private bool _hasAttacked;

    public EnemyAttackState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _hasAttacked = false;
        
        _fsm.rigid.linearVelocity = Vector2.zero;
        
        if (_fsm.animator != null)
        {
            _fsm.animator.SetBool("1_Move", false);
            _fsm.animator.SetTrigger("2_Attack");
        }
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (!_hasAttacked && _timer >= _fsm.data.attackDuration * 0.5f)
        {
            ExecuteSimpleAttack();
            _hasAttacked = true;
        }

        if (_timer >= _fsm.data.attackDuration)
        {
            _fsm.ChangeState(EnemyStateType.PostAttack);
        }
    }

    private void ExecuteSimpleAttack()
    {
        float distance = Vector2.Distance(_fsm.transform.position, _fsm.playerTransform.position);
        if (distance <= _fsm.data.attackRange)
        {
            if (_fsm.playerTransform.TryGetComponent(out Damageable d))
            {
                d.TakeDamage(_fsm.data.attackDamage);
            }
        }
    }

    public override void Exit() { }
}