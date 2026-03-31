using UnityEngine;

[CreateAssetMenu(fileName = "FinalBossData", menuName = "Monster/Final Boss Data")]
public class FinalBossData : ScriptableObject
{
    [Header("기본 스탯")]
    public int maxHp        = 1000;
    public float moveSpeed  = 2f;
    public int attackDamage = 25;

    [Header("범위")]
    public float detectRange = 12f;
    public float attackRange = 3f;

    [Header("타이밍 (초)")]
    public float hitDuration    = 0.2f;
    public float attackDuration = 0.8f;
    public float attackCooldown = 1.5f;

    [Header("페이즈 전환")]
    [Range(0f, 1f)]
    public float phase2HpRatio = 0.5f;
    public float phase2SpeedMultiplier  = 1.5f;
    public int   phase2DamageMultiplier = 2;

    [Header("보스 스킬")]
    [Tooltip("워닝사인 지속 시간 (초)")]
    public float warningDuration = 1.5f;
    [Tooltip("공격 애니메이션 중 데미지 판정 타이밍 (초)")]
    public float skillFireTime   = 0.4f;
}