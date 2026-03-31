using System.Collections.Generic;
using UnityEngine;

public class BossFSM : MonoBehaviour, Damageable
{
    [Header("데이터 및 설정")]
    public BossData data;
    public GameObject warningSignObj;

    public Rigidbody2D rigid { get; private set; }
    public Animator animator { get; private set; }
    public EnemyPathfinder pathfinder { get; private set; }
    private EnemyEffectManager _effectManager;

    public Boss1Skill boss1Skill { get; private set; }
    public Boss2Skill boss2Skill { get; private set; }
    public Boss3Skill4 boss3Skill4 { get; private set; }
    public Boss3Skill10 boss3Skill10 { get; private set; }

    private Dictionary<BossStateType, BossStateBase> _states;
    private BossStateBase _currentState;
    public BossStateType currentStateType { get; private set; }

    public Transform playerTransform { get; private set; }
    public MonsterSkillDataSO selectedSkill { get; set; }
    public Vector2 initialTargetPos { get; set; }
    
    public RoomNode currentRoom { get; private set; } 
    
    private int _currentHp;

    public bool isPlayerInDetectRange => data != null && playerTransform != null && 
        Vector2.Distance(transform.position, playerTransform.position) <= data.detectRange;
    public bool isPlayerInAttackRange => data != null && playerTransform != null && 
        Vector2.Distance(transform.position, playerTransform.position) <= data.attackRange;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        _effectManager = GetComponent<EnemyEffectManager>();
        pathfinder = GetComponent<EnemyPathfinder>();

        boss1Skill = GetComponent<Boss1Skill>();
        boss2Skill = GetComponent<Boss2Skill>();
        boss3Skill4 = GetComponent<Boss3Skill4>();
        boss3Skill10 = GetComponent<Boss3Skill10>();

        if (data != null) _currentHp = data.maxHp;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        _states = new Dictionary<BossStateType, BossStateBase>
        {
            { BossStateType.Idle,       new BossIdleState(this) },
            { BossStateType.Chase,      new BossChaseState(this) },
            { BossStateType.Warning,    new BossWarningState(this) },
            { BossStateType.Attack,     new BossAttackState(this) },
            { BossStateType.PostAttack, new BossPostAttackState(this) },
            { BossStateType.Hit,        new BossHitState(this) },
            { BossStateType.Die,        new BossDieState(this) }
        };
    }

    private void OnEnable()
    {
        if (_states != null) ChangeState(BossStateType.Idle);
    }

    protected virtual void Update() => _currentState?.Update();

    public void SetRoom(RoomNode room)
    {
        currentRoom = room;
        if (pathfinder != null && room != null)
            pathfinder.InitGrid(room);
    }

    public void ResetBoss()
    {
        if (data != null) _currentHp = data.maxHp;
        currentStateType = BossStateType.Idle;
        StopAllCoroutines();

        if (rigid != null) rigid.bodyType = RigidbodyType2D.Dynamic;

        if (animator != null)
        {
            animator.SetBool("1_Move", false);
            animator.SetBool("isDeath", false);
        }
    }

    public void ChangeState(BossStateType next)
    {
        if (currentStateType == BossStateType.Die) return;
        _currentState?.Exit();
        currentStateType = next;
        _currentState = _states[next];
        _currentState.Enter();
    }

    public virtual void TakeDamage(int damage)
    {
        if (_currentHp <= 0 || currentStateType == BossStateType.Die) return;
        
        _currentHp -= damage;
        if (_effectManager != null) _effectManager.PlayHitFlash(); 

        if (_currentHp <= 0)
        {
            ChangeState(BossStateType.Die);
            return;
        }

        if (currentStateType == BossStateType.Warning || currentStateType == BossStateType.Attack) return;

        animator?.SetTrigger("3_Hit");
        ChangeState(BossStateType.Hit);
    }


    public void TriggerAttackSkill()
    {
        if (selectedSkill == null) return;

        if (selectedSkill.skillId == 4 && boss3Skill4 != null)
        {
            boss3Skill4.ExecuteSkill(selectedSkill, initialTargetPos, data.attackDamage);
            return; 
        }
        
        if (selectedSkill.skillId == 10 && boss3Skill10 != null)
        {
            boss3Skill10.ExecuteSkill(selectedSkill, initialTargetPos, data.attackDamage);
            return;
        }

        if (selectedSkill.skillType == MonsterSkillType.Dash && boss1Skill != null)
            boss1Skill.ExecuteSkill(selectedSkill, initialTargetPos, data.attackDamage);
        else if (selectedSkill.skillType == MonsterSkillType.Melee && boss2Skill != null)
            boss2Skill.ExecuteSkill(selectedSkill, initialTargetPos, data.attackDamage);
    }

    public void StopAllSkills()
    {
        if (boss1Skill != null) boss1Skill.StopSkill();
        if (boss2Skill != null) boss2Skill.StopSkill();
        if (boss3Skill4 != null) boss3Skill4.StopSkill();
        if (boss3Skill10 != null) boss3Skill10.StopSkill();
    }

    public void SelectRandomSkill()
    {
        if (data != null && data.skills != null && data.skills.Count > 0)
            selectedSkill = data.skills[Random.Range(0, data.skills.Count)];
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
}