using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossFSM : MonoBehaviour, Damageable
{
    [Header("데이터")]
    public BossData data;
    [Header("워닝 시스템")]
    public GameObject warningSignObj; 

    private Rigidbody2D _rigid;
    private EnemyEffectManager _effectManager;
    private EnemyPathfinder _pathfinder;
    private MonsterSkillExecutor _skillExecutor;
    private Animator _animator;
    private Dictionary<BossStateType, BossStateBase> _states;
    private BossStateBase _currentState;
    private int _currentHp;

    public BossStateType currentStateType { get; private set; }
    public Transform playerTransform      { get; private set; }
    public Rigidbody2D rigid   => _rigid;
    public Animator    animator => _animator;
    public EnemyPathfinder pathfinder => _pathfinder;
    public MonsterSkillExecutor skillExecutor => _skillExecutor;
    public MonsterSkillDataSO selectedSkill { get; set; } 
    public Vector2 lockedFacingDir { get; set; } 

    public bool isPlayerInDetectRange => data != null && playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= data.detectRange;
    public bool isPlayerInAttackRange => data != null && playerTransform != null && Vector2.Distance(transform.position, playerTransform.position) <= data.attackRange;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _effectManager = GetComponent<EnemyEffectManager>();
        _pathfinder = GetComponent<EnemyPathfinder>();
        _skillExecutor = GetComponent<MonsterSkillExecutor>();
        _animator = GetComponentInChildren<Animator>();

        if (data != null) _currentHp = data.maxHp;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        _states = new Dictionary<BossStateType, BossStateBase>
        {
            { BossStateType.Idle,        new BossIdleState(this)        },
            { BossStateType.Chase,       new BossChaseState(this)       },
            { BossStateType.Warning,     new BossWarningState(this)     },
            { BossStateType.Attack,      new BossAttackState(this)      },
            { BossStateType.PostAttack,  new BossPostAttackState(this)  },
            { BossStateType.Hit,         new BossHitState(this)         },
            { BossStateType.Die,         new BossDieState(this)         }
        };
    }

    private void OnEnable()
    {
        if (warningSignObj != null) warningSignObj.SetActive(false);
        ChangeState(BossStateType.Idle);
    }

    private void Update() => _currentState?.Update();

    public void ChangeState(BossStateType next)
    {
        if (currentStateType == BossStateType.Die) return;
        _currentState?.Exit();
        currentStateType = next;
        _currentState = _states[next];
        _currentState.Enter();
    }

    public void SelectRandomSkill()
    {
        if (data != null && data.skills != null && data.skills.Count > 0)
            selectedSkill = data.skills[Random.Range(0, data.skills.Count)];
    }

    public void TriggerAttackSkill()
    {
        if (currentStateType == BossStateType.Attack && selectedSkill != null)
            skillExecutor.TryExecute(selectedSkill, transform, lockedFacingDir, data.attackDamage);
    }

    public void TakeDamage(int damage)
    {
        if (_currentHp <= 0) return;
        _currentHp -= damage;
        _effectManager?.PlayHitEffect();
        if (_currentHp <= 0) { ChangeState(BossStateType.Die); return; }
        if (currentStateType == BossStateType.Warning || currentStateType == BossStateType.Attack) return; 
        ChangeState(BossStateType.Hit);
    }

    public void FlipToPlayer()
    {
        if (playerTransform == null) return;
        float dirX = playerTransform.position.x - transform.position.x;
        Vector3 scale = transform.localScale;
        scale.x = dirX < 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}