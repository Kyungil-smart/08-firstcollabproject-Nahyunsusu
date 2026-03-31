using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomSpawnConfig", menuName = "Monster/Room Spawn Config")]
public class RoomSpawnConfigSO : ScriptableObject
{
    [Header("스폰 수 범위")]
    [Min(1)]
    public int minSpawnCount = 2;
    [Min(1)]
    public int maxSpawnCount = 5;

    [Header("스폰 가능한 몬스터 프리팹 목록")]
    public List<GameObject> enemyPrefabs;
}
