using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour, IDamageable
{
    [Header("데이터")]
    public EnemyData data;

    private Rigidbody2D _rigid;
    private EnemyEffectManager _effectManager;
    private Dictionary<EnemyStateType, EnemyStateBase> _states;
    private EnemyStateBase _currentState;
    private int _currentHp;
    private bool _isInvincible = false;

    public EnemyStateType currentStateType { get; private set; }
    public Transform playerTransform { get; private set; }
    public Rigidbody2D rigid => _rigid;

    public bool isPlayerInDetectRange =>
        playerTransform != null &&
        Vector2.Distance(transform.position, playerTransform.position) <= data.detectRange;

    public bool isPlayerInAttackRange =>
        playerTransform != null &&
        Vector2.Distance(transform.position, playerTransform.position) <= data.attackRange;

    private void Awake()
    {
        _rigid         = GetComponent<Rigidbody2D>();
        _effectManager = GetComponent<EnemyEffectManager>();

        if (data == null) return;

        _currentHp = data.maxHp;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        _states = new Dictionary<EnemyStateType, EnemyStateBase>
        {
            { EnemyStateType.Idle,       new EnemyIdleState(this)       },
            { EnemyStateType.Chase,      new EnemyChaseState(this)      },
            { EnemyStateType.Hit,        new EnemyHitState(this)        },
            { EnemyStateType.Attack,     new EnemyAttackState(this)     },
            { EnemyStateType.PostAttack, new EnemyPostAttackState(this) },
            { EnemyStateType.Die,        new EnemyDieState(this)        }
        };
    }

    private void OnEnable()
    {
        if (_states != null)
            ChangeState(EnemyStateType.Idle);
    }

    private void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(EnemyStateType next)
    {
        if (currentStateType == EnemyStateType.Die) return;

        _currentState?.Exit();
        currentStateType = next;
        _currentState    = _states[next];
        _currentState.Enter();
    }

    public void ResetEnemy()
    {
        _currentHp       = data.maxHp;
        _isInvincible    = false;
        currentStateType = EnemyStateType.Idle;
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;
        if (_currentHp <= 0) return;

        _currentHp -= damage;
        _effectManager?.PlayHitEffect();

        if (_currentHp <= 0)
        {
            ChangeState(EnemyStateType.Die);
            return;
        }

        StartCoroutine(InvincibleRoutine());
        ChangeState(EnemyStateType.Hit);
    }

    private IEnumerator InvincibleRoutine()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(0.2f);
        _isInvincible = false;

        // 사운드 담당자 연결 필요
        // AudioManager.Instance.PlaySFX("몬스터 피격 SFX");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.attackRange);
    }
#endif
}