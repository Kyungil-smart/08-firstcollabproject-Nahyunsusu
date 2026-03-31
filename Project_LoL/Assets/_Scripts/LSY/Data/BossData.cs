using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BossData", menuName = "Monster/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("기본 스탯")]
    public string monsterName;
    public int maxHp = 1000;
    public float moveSpeed = 2f;
    public int attackDamage = 25;

    [Header("범위")]
    public float detectRange = 12f;
    public float attackRange = 3f;

    [Header("타이밍 (초)")]
    public float hitDuration = 0.2f;
    public float attackDuration = 0.8f;
    public float attackCooldown = 1.5f;

    [Header("스킬 리스트")]
    public List<MonsterSkillDataSO> skills = new List<MonsterSkillDataSO>();
}