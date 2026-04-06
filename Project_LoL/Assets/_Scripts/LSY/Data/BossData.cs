using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossData", menuName = "Monster/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("식별 정보")]
    public int monsterId;

    [Header("기본 스탯")]
    public int maxHp = 1000;
    public int attackDamage = 25;
    public float attackSpeed = 1f; 
    public float moveSpeed = 2f;

    [Header("범위 설정")]
    public float detectRange = 12f;
    public float attackRange = 3f;

    [Header("보상")]
    public int goldReward = 100;
    public int expReward = 50;
    public int chestId = 0;

    [Header("스킬 리스트")]
    public List<MonsterSkillDataSO> skills = new List<MonsterSkillDataSO>();
}