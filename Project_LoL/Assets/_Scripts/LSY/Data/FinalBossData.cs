using UnityEngine;

[CreateAssetMenu(fileName = "FinalBossData", menuName = "Monster/FinalBossData")]
public class FinalBossData : ScriptableObject
{
    [Header("기본 식별 정보")]
    [Tooltip("기본키 (ID)")]
    public int MonsterDataId = 20221001;

    [Header("전투 스탯")]
    [Tooltip("체력")]
    public int MonsterHp = 3000;
    
    [Tooltip("공격력")]
    public int MonsterAttack;
    
    [Tooltip("공격속도")]
    public float MonsterAttackSpeed;

    [Header("탐지 및 사거리")]
    [Tooltip("감지거리")]
    public float MonsterMoveRange;
    
    [Tooltip("공격사거리")]
    public float MonsterAttackRange;

    [Header("보상 정보")]
    [Tooltip("지급 골드")]
    public int MonsterGiveGold;
    
    [Tooltip("지급 경험치")]
    public int MonsterGiveExp;
    
    [Tooltip("보물상자 ID")]
    public int MonsterGiveChestId;
}