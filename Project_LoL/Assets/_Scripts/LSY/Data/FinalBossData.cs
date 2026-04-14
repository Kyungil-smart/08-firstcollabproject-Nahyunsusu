using UnityEngine;

[CreateAssetMenu(fileName = "FinalBossData", menuName = "Monster/FinalBossData")]
public class FinalBossData : ScriptableObject
{
    [Header("식별 정보")]
    public int monsterDataId = 20221001;

    [Header("기본 스탯")]
    [Tooltip("체력")]
    public int monsterHp = 3000;

    [Tooltip("공격력")]
    public int monsterAttack;

    [Tooltip("가로(X축) 공격사거리")]
    public float monsterAttackRangeX = 15f;

    [Tooltip("세로(Y축) 공격사거리")]
    public float monsterAttackRangeY = 8f;
}