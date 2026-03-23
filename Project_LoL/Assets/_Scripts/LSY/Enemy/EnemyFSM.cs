using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    [Header("데이터")]
    public EnemyData data;

    private Rigidbody2D _rigid;
    private SpriteRenderer _spriteRenderer;
    private Dictionary<EnemyStateType, EnemyStateBase> _states;
    private EnemyStateBase _currentState;
    private int _currentHp;
    private bool _isInvincible = false;

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
        _rigid             = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (data == null)
        {
            return;
        }

        _currentHp = data.maxHp;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        _states = new Dictionary<EnemyStateType, EnemyStateBase>
        {
            { EnemyStateType.Idle,       new EnemyIdleState(this)       },
            { EnemyStateType.Chase,      new EnemyChaseState(this)      },
            { EnemyStateType.Hit,        new EnemyHitState(this)        },
            { EnemyStateType.Attack,     new EnemyAttackState(this)     },
            { EnemyStateType.PostAttack, new EnemyPostAttackState(this) }
        };
    }

    private void Start()
    {
        ChangeState(EnemyStateType.Idle);
    }

    private void Update()
    {
        _currentState?.Update();
    }

    public void ChangeState(EnemyStateType next)
    {
        _currentState?.Exit();
        _currentState = _states[next];
        _currentState.Enter();
    }

    public void OnHit(int damage)
    {
        if (_isInvincible) return;

        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        _isInvincible = true;

        if (_spriteRenderer != null)
        {
            Color original = _spriteRenderer.color;
            _spriteRenderer.color = new Color(0.5f, original.g, original.b, original.a);
            yield return new WaitForSeconds(0.2f);
            _spriteRenderer.color = original;
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }

        _isInvincible = false;

        // 사운드 담당자 연결 필요
        // 몬스터 피격 SFX

        ChangeState(EnemyStateType.Hit);
    }

    private void Die()
    {

        // 경험치 매니저 연결 필요

        // 골드 매니저 연결 필요

        // 던전 퇴장 시 결과 UI에 표시될 처치 수 기록

        Destroy(gameObject);
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