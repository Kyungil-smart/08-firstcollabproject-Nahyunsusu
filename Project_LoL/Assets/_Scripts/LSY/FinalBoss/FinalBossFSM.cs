using UnityEngine;
using System.Collections;

public enum FinalBossState
{
    Idle,         
    SkillDelay,   
    SkillExecute, 
    Dead          
}

public class FinalBossFSM : MonoBehaviour, Damageable
{
    [Header("데이터")]
    public FinalBossData bossData;

    [Header("애니메이터")]
    [Tooltip("보스의 Animator")]
    public Animator animator;

    [Header("전투 설정")]
    [Tooltip("스킬 발동 직후 다음 스킬까지의 대기 시간")]
    public float skillDelayTime = 15f;

    [Header("스폰/위치 설정")]
    [Tooltip("보스 주변에 스킬이 생성되지 않는 안전 거리")]
    public float bossBlockRange = 4f;

    private int  _currentHp;
    private bool _phase50;
    private bool _phase30;
    private bool _phase20;
    private bool _phase10;

    private FinalBossState       _currentState;
    private FinalBossSkillBase[] _skills;
    private Coroutine            _skillDelayCoroutine;

    private Transform _playerTransform;

    public FinalBossState currentState    => _currentState;
    public Transform      playerTransform => _playerTransform;
    public int            currentHp       => _currentHp;
    public float          hpRatio         => bossData != null ? (float)_currentHp / bossData.monsterHp : 0f;
    
    public int            baseAttack      => bossData != null ? bossData.monsterAttack : 0;
    public float          attackRange     => bossData != null ? bossData.monsterAttackRange : 0f;

    public bool           isPhase50       => _phase50;
    public bool           isPhase30       => _phase30;
    public bool           isPhase20       => _phase20;
    public bool           isPhase10       => _phase10;

    private void Awake()
    {
        _skills = GetComponents<FinalBossSkillBase>();
        
        if (animator == null) 
            animator = GetComponentInChildren<Animator>();

        if (bossData != null)
        {
            _currentHp = bossData.monsterHp;
        }
        else
        {
            _currentHp = 3000;
        }

        ChangeState(FinalBossState.Idle);
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    private void Update()
    {
        if (_currentState == FinalBossState.Idle && _playerTransform != null)
        {
            Vector2 toPlayer = (Vector2)_playerTransform.position - (Vector2)transform.position;
            float dist = toPlayer.magnitude;
            
            if (dist <= attackRange)
            {
                Vector2 dirToPlayer = toPlayer.normalized;
                
                if (Vector2.Dot(Vector2.down, dirToPlayer) >= 0f)
                {
                    StartBattle();
                }
            }
        }
    }

    private void ChangeState(FinalBossState newState)
    {
        _currentState = newState;

        if (animator != null)
        {
            switch (_currentState)
            {
                case FinalBossState.Idle:
                case FinalBossState.SkillDelay:
                    animator.SetTrigger("doIdle");
                    break;
                case FinalBossState.SkillExecute:
                    animator.SetTrigger("doAttack");
                    break;
                case FinalBossState.Dead:
                    animator.SetTrigger("doDie");
                    break;
            }
        }
    }

    public void StartBattle()
    {
        if (_currentState != FinalBossState.Idle) return;
        
        ChangeState(FinalBossState.SkillDelay);
        StartSkillDelayTimer();
    }

    private void StartSkillDelayTimer()
    {
        if (_skillDelayCoroutine != null) 
            StopCoroutine(_skillDelayCoroutine);
        
        _skillDelayCoroutine = StartCoroutine(SkillDelayRoutine());
    }

    private IEnumerator SkillDelayRoutine()
    {
        yield return new WaitForSeconds(skillDelayTime);

        if (_currentState != FinalBossState.Dead)
            ExecuteRandomSkill();
    }

    private void ExecuteRandomSkill()
    {
        if (_skills == null || _skills.Length == 0) return;

        int index = Random.Range(0, _skills.Length);
        
        ChangeState(FinalBossState.SkillExecute);
        StartSkillDelayTimer();

        _skills[index].Execute(this, OnSkillFinished);
    }

    private void OnSkillFinished()
    {
        if (_currentState != FinalBossState.Dead)
        {
            ChangeState(FinalBossState.SkillDelay);
        }
    }

    public void TakeDamage(int damage)
    {
        if (_currentState == FinalBossState.Dead) return;

        _currentHp = Mathf.Max(0, _currentHp - damage);
        CheckHpPhases();

        if (_currentHp <= 0)
        {
            StopAllCoroutines();
            StopAllSkillCoroutines();
            ChangeState(FinalBossState.Dead);
            OnDead();
        }
    }

    private void CheckHpPhases()
    {
        if (!_phase50 && hpRatio <= 0.5f) _phase50 = true;
        if (!_phase30 && hpRatio <= 0.3f) _phase30 = true;
        if (!_phase20 && hpRatio <= 0.2f) _phase20 = true;
        if (!_phase10 && hpRatio <= 0.1f) _phase10 = true;
    }

    private void StopAllSkillCoroutines()
    {
        if (_skills == null) return;
        foreach (FinalBossSkillBase skill in _skills)
        {
            if (skill != null) skill.StopAllCoroutines();
        }
    }

    private void OnDead()
    {
        Debug.Log("클리어!");
    }

    public Vector2 GetRandomRoomPosition()
    {
        Vector2 pos;
        int maxTries = 100;
        
        do
        {
            float angle = Random.Range(180f, 360f) * Mathf.Deg2Rad;
            float radius = Random.Range(0f, attackRange);

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            pos = (Vector2)transform.position + new Vector2(x, y);
            maxTries--;
        }
        while (IsInBossBlockZone(pos) && maxTries > 0);

        return pos;
    }

    public bool IsInBossBlockZone(Vector2 pos)
    {
        Vector2 bossPos = transform.position;
        return Mathf.Abs(pos.x - bossPos.x) <= bossBlockRange &&
               Mathf.Abs(pos.y - bossPos.y) <= bossBlockRange;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (bossData == null) return;

        Vector2 pos = transform.position;
        float range = bossData.monsterAttackRange;

        Vector3 startDir = Quaternion.Euler(0, 0, -90) * Vector2.down;

        UnityEditor.Handles.color = new Color(1f, 0f, 0f, 0.15f);
        UnityEditor.Handles.DrawSolidArc(pos, Vector3.forward, startDir, 180f, range);

        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireArc(pos, Vector3.forward, startDir, 180f, range);
    }
#endif
}