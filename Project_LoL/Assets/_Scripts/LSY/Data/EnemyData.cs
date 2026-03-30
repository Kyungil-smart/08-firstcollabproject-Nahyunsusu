using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Monster/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("기본 스탯")]
    public int maxHp        = 100;
    public float moveSpeed  = 3f;
    public int attackDamage = 20;

    [Header("범위")]
    public float detectRange = 8f;
    public float attackRange = 2f;

    [Header("타이밍 (초)")]
    public float hitDuration    = 0.3f;
    public float attackDuration = 1.0f;
    public float attackCooldown = 1f;

    [Header("스킬")]
    public MonsterSkillDataSO skill;

    [Header("드랍")]
    public int expReward  = 10;
    public int goldReward = 5;
}
