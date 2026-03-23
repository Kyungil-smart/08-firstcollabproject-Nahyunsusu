using UnityEngine;

public class EnemyAttackState : EnemyStateBase
{
    private float _timer;
    private bool _hasDealtDamage;

    public EnemyAttackState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _hasDealtDamage = false;
        _fsm.rigid.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        _timer += Time.deltaTime;

        if (!_hasDealtDamage && _timer >= _fsm.data.attackDuration * 0.5f)
        {
            TryDealDamage();
            _hasDealtDamage = true;
        }

        if (_timer >= _fsm.data.attackDuration)
            _fsm.ChangeState(EnemyStateType.PostAttack);
    }

    public override void Exit() { }

    private void TryDealDamage()
    {
        if (!_fsm.isPlayerInAttackRange) return;
        if (_fsm.playerTransform.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(_fsm.data.attackDamage);
    }
}