using System.Collections;
using UnityEngine;

public class Boss1Skill : BossSkillBase
{
    private Rigidbody2D _rb;
    private Animator _animator;
    
    private bool _isDashing = false;
    private Vector2 _dashDir;
    private float _dashSpeed;
    private float _dashRange;
    private Vector2 _startPos;
    
    private float _trailTimer;
    private bool _useVfxA = true;
    private bool _hasHit = false;
    private MonsterSkillDataSO _currentSkill;
    private int _finalDamage;
    private float _dashTimer; 

    private GameObject _warningObj;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    public override void ExecuteSkill(MonsterSkillDataSO skill, Vector2 targetPos, int baseDamage)
    {
        if (skill == null) return;
        
        _currentSkill = skill;
        _finalDamage = skill.baseDamage + baseDamage;
        _dashSpeed = skill.projectileSpeed > 0 ? skill.projectileSpeed : 10f;
        _dashRange = skill.range > 0 ? skill.range : 5f;
        _hasHit = false;

        _dashDir = (targetPos - (Vector2)transform.position).normalized;
        if (_dashDir == Vector2.zero) _dashDir = transform.right;

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        Vector2 warnStartPos = transform.position;
        float angle = Mathf.Atan2(_dashDir.y, _dashDir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        if (_currentSkill.warningPrefab != null && _currentSkill.warningDuration > 0)
        {
            _warningObj = SkillPool.Instance.Spawn(_currentSkill.warningPrefab, warnStartPos, rot);
            
            float timer = 0f;
            while (timer < _currentSkill.warningDuration)
            {
                if (_warningObj == null || !_warningObj.activeInHierarchy) yield break;
                timer += Time.deltaTime;
                
                float currentLength = Mathf.Lerp(0, _dashRange, timer / _currentSkill.warningDuration);
                _warningObj.transform.localScale = new Vector3(currentLength, _currentSkill.damageRangeY, 1f);
                
                _warningObj.transform.position = (Vector2)transform.position + (_dashDir * (currentLength * 0.5f));
                
                yield return null;
            }
            if (_warningObj != null) 
            {
                SkillPool.Instance.Despawn(_warningObj);
                _warningObj = null;
            }
        }
        else
        {
            yield return new WaitForSeconds(_currentSkill.warningDuration);
        }

        _startPos = _rb.position; 
        _isDashing = true; 
        _dashTimer = 0f; 

        if (_animator != null) _animator.SetBool("1_Move", true); 
    }

    private void FixedUpdate()
    {
        if (!_isDashing) return;

        _rb.linearVelocity = _dashDir * _dashSpeed;

        _trailTimer += Time.fixedDeltaTime;
        if (_trailTimer >= 0.1f)
        {
            GameObject smokePrefab = _useVfxA ? _currentSkill.trailVfxPrefabA : _currentSkill.trailVfxPrefabB;
            if (smokePrefab != null) 
            {
                GameObject trail = SkillPool.Instance.Spawn(smokePrefab, transform.position, Quaternion.identity);
                SkillPool.Instance.Despawn(trail, 1.0f);
            }
            _useVfxA = !_useVfxA;
            _trailTimer = 0f;
        }

        if (!_hasHit)
        {
            float angle = Mathf.Atan2(_dashDir.y, _dashDir.x) * Mathf.Rad2Deg;
            Collider2D hit = Physics2D.OverlapBox(_rb.position, new Vector2(_currentSkill.damageRangeX, _currentSkill.damageRangeY), angle, _currentSkill.targetLayer);

            if (hit != null && hit.TryGetComponent(out Damageable target))
            {
                if (!hit.CompareTag("Enemy") && !hit.CompareTag("Boss"))
                {
                    target.TakeDamage(_finalDamage);
                    if (_currentSkill.hitVfxPrefab != null) 
                    {
                        GameObject vfx = SkillPool.Instance.Spawn(_currentSkill.hitVfxPrefab, hit.transform.position, Quaternion.identity);
                        SkillPool.Instance.Despawn(vfx, 1.0f);
                    }
                    _hasHit = true; 
                }
            }
        }

        _dashTimer += Time.fixedDeltaTime;
        float maxDashTime = (_dashRange / Mathf.Max(0.1f, _dashSpeed)) + 0.5f;

        if (Vector2.Distance(_startPos, _rb.position) >= _dashRange || _dashTimer > maxDashTime)
        {
            StopSkill();
        }
    }

    public override void StopSkill()
    {
        base.StopSkill(); 
        if (_warningObj != null) 
        {
            SkillPool.Instance.Despawn(_warningObj);
            _warningObj = null;
        }
        
        if (_isDashing)
        {
            _isDashing = false;
            _rb.linearVelocity = Vector2.zero;
            if (_animator != null) _animator.SetBool("1_Move", false);
        }
    }
}