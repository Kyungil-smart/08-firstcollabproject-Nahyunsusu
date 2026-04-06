using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Monster/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("식별 정보")]
    public int monsterId;

    [Header("기본 스탯")]
    public int maxHp = 100;
    public int attackDamage = 20;
    public float attackSpeed = 1f; 
    public float moveSpeed = 3f;

    [Header("범위 설정")]
    public float detectRange = 8f;
    public float attackRange = 2f;

    [Header("보상")]
    public int goldReward = 10;
    public int expReward = 10;
    public int chestId = 0;

    [Header("스킬")]
    public MonsterSkillDataSO skill;
}