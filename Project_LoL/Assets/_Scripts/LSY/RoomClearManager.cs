using System.Collections.Generic;
using UnityEngine;

public class RoomClearManager : MonoBehaviour
{
    public static RoomClearManager Instance { get; private set; }

    [Header("참조")]
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private RoomCombatHandler _roomCombatHandler;
    [SerializeField] private EnemyPool _enemyPool;
    [SerializeField] private RoomSpawnConfigSO _spawnConfig;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartRoom(RoomNode room)
    {
        if (room == null)
        {
            Debug.LogWarning("[RoomClearManager] StartRoom 실패: room이 null입니다.");
            return;
        }

        if (room.roomData == null || room.roomData.roomType != RoomType.Combat)
            return;

        if (_roomCombatHandler == null || _enemyPool == null || _spawnConfig == null)
        {
            Debug.LogWarning("[RoomClearManager] 필요한 매니저나 설정(SO)이 연결되지 않았습니다.");
            return;
        }

        if (_spawnConfig.enemyPrefabs == null || _spawnConfig.enemyPrefabs.Count == 0)
        {
            Debug.LogWarning("[RoomClearManager] RoomSpawnConfigSO에 등록된 프리팹이 없습니다.");
            return;
        }

        int totalSpawnCount = Random.Range(_spawnConfig.minSpawnCount, _spawnConfig.maxSpawnCount + 1);

        List<Vector2> positions = _roomCombatHandler.GetSpawnPositions(room, totalSpawnCount);

        if (positions == null || positions.Count == 0)
        {
            Debug.LogWarning($"[RoomClearManager] {room.nodeId} 스폰 위치가 없습니다.");
            return;
        }

        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);
        if (runtimeData == null)
        {
            Debug.LogWarning($"[RoomClearManager] {room.nodeId} RoomRuntimeData가 없습니다.");
            return;
        }

        runtimeData.StartCombat(positions.Count);

        for (int i = 0; i < positions.Count; i++)
        {
            int index = Random.Range(0, _spawnConfig.enemyPrefabs.Count);
            _enemyPool.Spawn(_spawnConfig.enemyPrefabs[index], positions[i], room);
        }

        Debug.Log($"[RoomClearManager] {room.nodeId} 전투 시작 / 몬스터 수: {positions.Count}");
    }

    public void OnEnemyDied(RoomNode room)
    {
        if (room == null) return;

        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);
        if (runtimeData == null) return;

        runtimeData.OnMonsterDead();

        if (runtimeData.state == RoomState.Cleared)
        {
            Debug.Log($"[RoomClearManager] {room.nodeId} 클리어! 문 개방 신호 전송");
            _mapManager?.OnCombatCleared(room);
        }
    }
}