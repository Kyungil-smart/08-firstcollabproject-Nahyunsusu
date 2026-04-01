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

        // 1. 이번 방에 소환할 '전체 마릿수' 계산
        int totalTargetCount = 0;
        foreach (var config in poolConfigs)
        {
            totalTargetCount += config.spawnAmount;
        }

        // 2. JJH님 핸들러에게 전체 마릿수만큼 '안전한 위치' 요청
        List<Vector2> positions = _roomCombatHandler.GetSpawnPositions(room, totalTargetCount);
        
        // 3. 실제 확보된 위치 개수 (맵이 좁으면 설정보다 적게 나올 수 있음)
        int actualSpawnCount = positions.Count;

        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);

        if (actualSpawnCount > 0)
        {
            // 전투 시작 알림 (실제 소환될 마릿수 기준)
            runtimeData?.StartCombat(actualSpawnCount);

            int currentPosIndex = 0;

            // 4. 리스트에 있는 모든 종류의 몬스터를 순서대로 소환
            foreach (var config in poolConfigs)
            {
                for (int i = 0; i < config.spawnAmount; i++)
                {
                    // 위치를 다 썼으면 중단 (맵 크기 제한)
                    if (currentPosIndex >= actualSpawnCount) break;

                    EnemyPool.Instance.Spawn(config.prefab, positions[currentPosIndex], room);
                    currentPosIndex++;
                }
            }
        }
        else
        {
            // 예외처리: 스폰할 자리가 아예 없다면 즉시 문을 열어줌
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