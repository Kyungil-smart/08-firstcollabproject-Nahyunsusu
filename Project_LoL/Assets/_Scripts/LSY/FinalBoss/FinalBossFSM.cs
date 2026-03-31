using System.Collections;
using UnityEngine;

public class FinalBossFSM : MonoBehaviour, Damageable
{
    [Header("데이터 및 설정")]
    [SerializeField] private FinalBossData data; 
    
    [Header("상태 정보")]
    private int _currentHp;
    private float _skillTimer;
    private RoomNode _currentRoom;

    private Animator _animator;
    private EnemyEffectManager _effectManager;
    private FinalBossSkill1 _skill1;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _effectManager = GetComponent<EnemyEffectManager>();
        _skill1 = GetComponent<FinalBossSkill1>();

        if (data != null)
        {
            _currentHp = data.maxHp;
            _skillTimer = 15f;
        }
    }

    public void SetRoom(RoomNode room) => _currentRoom = room;

    private void Update()
    {
        if (_currentHp <= 0) return;

        _skillTimer -= Time.deltaTime;
        if (_skillTimer <= 0)
        {
            TriggerBossSkill();
            _skillTimer = 15f; 
        }
    }

    public void TakeDamage(int damage)
    {
        if (_currentHp <= 0) return;

        _currentHp -= damage;
        _effectManager?.PlayHitFlash();

        if (_currentHp <= 0)
        {
            Die();
            return;
        }

        TriggerBossSkill();
    }

    private void TriggerBossSkill()
    {
        if (_skill1 == null) return;

        _animator?.SetTrigger("Attack");

        bool isLowHp = ((float)_currentHp / data.maxHp) <= 0.5f;
        _skill1.Execute(_currentRoom, isLowHp);
    }

    private void Die()
    {
        _animator?.SetBool("isDeath", true);
        RoomClearManager.Instance.OnFinalBossDied(_currentRoom);
        Destroy(gameObject, 2.5f);
    }
}