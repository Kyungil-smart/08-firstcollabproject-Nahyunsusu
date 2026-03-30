using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : MonoBehaviour, Damageable
{
    [Header("데이터")]
    public EnemyData data;

    [Header("애니메이터")]
    [SerializeField] private Animator _animatorRef;

    private Rigidbody2D _rigid;
    private EnemyEffectManager _effectManager;
    private EnemyPathfinder _pathfinder;
    private MonsterSkillExecutor _skillExecutor;
    private Animator _animator;
    private Dictionary<EnemyStateType, EnemyStateBase> _states;
    private EnemyStateBase _currentState;
    private int _currentHp;

    public EnemyStateType currentStateType { get; private set; }
    public Transform playerTransform { get; private set; }
    public Rigidbody2D rigid => _rigid;
    public Animator animator => _animator;
    public EnemyPathfinder pathfinder => _pathfinder;
    public MonsterSkillExecutor skillExecutor => _skillExecutor;
    public RoomNode currentRoom { get; private set; }

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
        _pathfinder    = GetComponent<EnemyPathfinder>();
        _skillExecutor = GetComponent<MonsterSkillExecutor>();

        _animator = _animatorRef != null
            ? _animatorRef
            : GetComponentInChildren<Animator>();

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

    public void SetRoom(RoomNode room)
    {
        currentRoom = room;
        if (_pathfinder != null && room != null)
            _pathfinder.InitGrid(room);
    }

    public void ResetEnemy()
    {
        _currentHp       = data.maxHp;
        currentStateType = EnemyStateType.Idle;
        StopAllCoroutines();

        if (_animator != null)
        {
            _animator.SetBool("1_Move",  false);
            _animator.SetBool("isDeath", false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_currentHp <= 0) return;

        _currentHp -= damage;
        _effectManager?.PlayHitEffect();

        if (_currentHp <= 0)
        {
            ChangeState(EnemyStateType.Die);
            return;
        }

        // 사운드 담당자 연결 필요
        // AudioManager.Instance.PlaySFX("몬스터 피격 SFX");

        if (currentStateType == EnemyStateType.Attack)
        {
            return;
        }

        ChangeState(EnemyStateType.Hit);
    }

    public void FlipToPlayer()
    {
        if (playerTransform == null) return;

        float dirX = playerTransform.position.x - transform.position.x;
        if (dirX == 0) return;

        Vector3 scale = transform.localScale;
        scale.x = dirX < 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
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