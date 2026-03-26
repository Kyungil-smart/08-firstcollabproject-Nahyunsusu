using System;
using System.Collections.Generic;
using UnityEngine;

// 방 전투 진행 관리
// 몬스터 스폰 → 카운트 관리 → 전투 종료 판단 → JJH에게 클리어 신호 전달
public class RoomClearManager : MonoBehaviour
{
    public static RoomClearManager Instance { get; private set; }

    [Header("참조")]
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private RoomCombatHandler _roomCombatHandler;
    [SerializeField] private EnemyPool _enemyPool;

    // 방 클리어 시 JJH에게 전달할 이벤트
    public event Action<RoomNode> OnRoomCleared;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // JJH가 방 입장 시 호출
    public void StartRoom(RoomNode room)
    {
        if (room == null)
        {
            Debug.LogWarning("[RoomClearManager] StartRoom 실패: room이 null입니다.");
            return;
        }

        if (room.roomData == null || room.roomData.roomType != RoomType.Combat)
            return;

        if (_roomCombatHandler == null)
        {
            Debug.LogWarning("[RoomClearManager] RoomCombatHandler가 연결되지 않았습니다.");
            return;
        }

        if (_enemyPool == null)
        {
            Debug.LogWarning("[RoomClearManager] EnemyPool이 연결되지 않았습니다.");
            return;
        }

        if (_enemyPool.poolConfigs == null || _enemyPool.poolConfigs.Count == 0)
        {
            Debug.LogWarning("[RoomClearManager] EnemyPool에 등록된 프리팹이 없습니다.");
            return;
        }

        // poolConfigs의 initialSize 합산 → 총 스폰 수
        int totalSpawnCount = 0;
        foreach (EnemyPool.PoolConfig config in _enemyPool.poolConfigs)
            totalSpawnCount += config.initialSize;

        // 스폰 위치 계산 (JJH)
        List<Vector2> positions = _roomCombatHandler.GetSpawnPositions(room, totalSpawnCount);

        if (positions == null || positions.Count == 0)
        {
            Debug.LogWarning($"[RoomClearManager] {room.nodeId} 스폰 위치가 없습니다.");
            return;
        }

        // RoomRuntimeData 전투 시작 처리
        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);
        if (runtimeData == null)
        {
            Debug.LogWarning($"[RoomClearManager] {room.nodeId} RoomRuntimeData가 없습니다.");
            return;
        }

        runtimeData.StartCombat(positions.Count);

        // poolConfigs 순서대로 initialSize만큼 스폰
        int posIndex = 0;
        foreach (EnemyPool.PoolConfig config in _enemyPool.poolConfigs)
        {
            for (int i = 0; i < config.initialSize; i++)
            {
                if (posIndex >= positions.Count) break;
                _enemyPool.Spawn(config.prefab, positions[posIndex], room);
                posIndex++;
            }
        }

        Debug.Log($"[RoomClearManager] {room.nodeId} 전투 시작 / 몬스터 수: {positions.Count}");
    }

    // 몬스터 사망 시 EnemyDieState에서 호출
    public void OnEnemyDied(RoomNode room)
    {
        if (room == null) return;

        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);
        if (runtimeData == null) return;

        runtimeData.OnMonsterDead();

        if (runtimeData.state == RoomState.Cleared)
        {
            Debug.Log($"[RoomClearManager] {room.nodeId} 클리어!");
            OnRoomCleared?.Invoke(room);
        }
    }
}