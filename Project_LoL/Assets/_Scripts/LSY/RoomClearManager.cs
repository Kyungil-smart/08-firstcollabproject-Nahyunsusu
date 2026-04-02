using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomClearManager : MonoBehaviour
{
    public static RoomClearManager Instance { get; private set; }

    private MapManager _mapManager;
    private RoomCombatHandler _roomCombatHandler;

    public static event Action<int, int> OnRewardDropped;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _mapManager = FindAnyObjectByType<MapManager>();
        _roomCombatHandler = FindAnyObjectByType<RoomCombatHandler>();
    }

    public void StartRoom(RoomNode room)
    {
        if (room == null || room.roomData == null) return;

        if (room.roomData.roomType == RoomType.Combat)
        {
            SpawnNormalEnemies(room);
        }
    }

    private void SpawnNormalEnemies(RoomNode room)
    {
        var poolConfigs = EnemyPool.Instance.poolConfigs;
        if (poolConfigs == null || poolConfigs.Count == 0) return;

        int totalTargetCount = 0;
        foreach (var config in poolConfigs)
        {
            totalTargetCount += config.spawnAmount;
        }

        List<Vector2> positions = _roomCombatHandler.GetSpawnPositions(room, totalTargetCount);
        
        int actualSpawnCount = positions.Count;

        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);

        if (actualSpawnCount > 0)
        {
            runtimeData?.StartCombat(actualSpawnCount);

            int currentPosIndex = 0;

            foreach (var config in poolConfigs)
            {
                for (int i = 0; i < config.spawnAmount; i++)
                {
                    if (currentPosIndex >= actualSpawnCount) break;

                    EnemyPool.Instance.Spawn(config.prefab, positions[currentPosIndex], room);
                    currentPosIndex++;
                }
            }
        }
        else
        {
            Debug.LogWarning($"[RoomClearManager] {room.roomData.name} 방에 스폰 공간이 없습니다!");
            ForceClearRoom(room, runtimeData);
        }
    }

    private void ForceClearRoom(RoomNode room, RoomRuntimeData runtimeData)
    {
        if (runtimeData != null)
        {
            runtimeData.StartCombat(1);
            runtimeData.OnMonsterDead();
        }
        _mapManager?.OnCombatCleared(room);
    }

    public void OnEnemyDied(RoomNode room, int gold, int exp)
    {
        OnRewardDropped?.Invoke(gold, exp); 
        UpdateProgress(room);
    }

    private void UpdateProgress(RoomNode room)
    {
        if (room == null) return;

        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);
        if (runtimeData != null)
        {
            runtimeData.OnMonsterDead();
            
            if (runtimeData.state == RoomState.Cleared) 
            {
                _mapManager?.OnCombatCleared(room);
            }
        }
    }
}