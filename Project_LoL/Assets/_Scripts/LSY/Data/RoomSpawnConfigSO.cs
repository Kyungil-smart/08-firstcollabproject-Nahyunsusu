using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterPoolData
{
    [Tooltip("스폰 목록 ID")]
    public int monsterPoolId;

    [Tooltip("실제 소환할 프리팹")]
    public GameObject monsterPrefab;

    [Tooltip("몬스터 데이터 ID")]
    public int monsterDataId;

    [Tooltip("등장 스테이지")]
    public int stage;

    [Tooltip("한 방에 등장할 마릿수")]
    public int amount;
}

[CreateAssetMenu(fileName = "RoomSpawnConfig", menuName = "Monster/Room Spawn Config")]
public class RoomSpawnConfigSO : ScriptableObject
{
    [Header("몬스터 스폰")]
    public List<MonsterPoolData> monsterPoolTable;
}