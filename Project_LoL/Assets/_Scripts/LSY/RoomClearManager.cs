using System.Collections.Generic;
using UnityEngine;

public class RoomClearManager : MonoBehaviour
{
    public static RoomClearManager Instance { get; private set; }

    [Header("참조")]
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private RoomCombatHandler _roomCombatHandler;
    [SerializeField] private RoomSpawnConfigSO _spawnConfig;

    [Header("최종 보스 설정")]
    [SerializeField] private GameObject _finalBossPrefab;

    [Header("현재 스테이지")]
    public int currentStage = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartRoom(RoomNode room)
    {
        if (room == null || room.roomData == null) return;

        if (room.roomData.roomType == RoomType.Boss && currentStage == 2)
        {
            SpawnFinalBoss(room);
        }
        else if (room.roomData.roomType == RoomType.Combat)
        {
            SpawnNormalEnemies(room);
        }
    }

    private void SpawnFinalBoss(RoomNode room)
    {
        if (_finalBossPrefab == null) return;

        Vector2 center = room.GetBounds().center;
        Vector3 spawnPos = new Vector3(center.x, center.y, 0);

        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);
        runtimeData?.StartCombat(1);

        GameObject bossObj = Instantiate(_finalBossPrefab, spawnPos, Quaternion.identity);
        if (bossObj.TryGetComponent(out FinalBossFSM finalBoss))
        {
            finalBoss.SetRoom(room);
        }
    }

    private void SpawnNormalEnemies(RoomNode room)
    {
        List<MonsterPoolData> candidates = _spawnConfig.monsterPoolTable.FindAll(x => x.stage == currentStage);
        if (candidates.Count == 0) return;

        MonsterPoolData selected = candidates[Random.Range(0, candidates.Count)];
        List<Vector2> positions = _roomCombatHandler.GetSpawnPositions(room, selected.amount);

        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);
        runtimeData?.StartCombat(positions.Count);

        foreach (var pos in positions)
        {
            EnemyPool.Instance.Spawn(selected.monsterPrefab, pos, room);
        }
    }

    public void OnFinalBossDied(RoomNode room)
    {
        
        UpdateProgress(room);
    }

    public void OnEnemyDied(RoomNode room) => UpdateProgress(room);

    private void UpdateProgress(RoomNode room)
    {
        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);
        if (runtimeData != null)
        {
            runtimeData.OnMonsterDead();
            if (runtimeData.state == RoomState.Cleared) _mapManager?.OnCombatCleared(room);
        }
    }
}