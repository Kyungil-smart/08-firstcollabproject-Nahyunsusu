using System.Collections.Generic;
using UnityEngine;

public class RoomClearManager : MonoBehaviour
{
    public static RoomClearManager Instance { get; private set; }

    private MapManager _mapManager;
    private RoomCombatHandler _roomCombatHandler;
    private bool _isBossStarted = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _mapManager = FindAnyObjectByType<MapManager>();
        _roomCombatHandler = FindAnyObjectByType<RoomCombatHandler>();
    }

    private void Start()
    {
        Invoke("HookBossTrigger", 1.0f);
    }

    private void HookBossTrigger()
    {
        GameObject triggerObj = GameObject.Find("BossTrigger");
        
        if (triggerObj != null)
        {
            RoomTrigger rt = triggerObj.GetComponent<RoomTrigger>();
            if (rt != null)
            {
                BossRoomDetector detector = triggerObj.AddComponent<BossRoomDetector>();
                detector.Setup(rt.room, this);
            }
        }
    }

    public void StartRoom(RoomNode room)
    {
        if (room == null || room.roomData == null) return;

        if (room.roomData.roomType == RoomType.Combat)
            SpawnNormalEnemies(room);
        else if (room.roomData.roomType == RoomType.Boss)
            SpawnBoss(room);
    }

    private void SpawnNormalEnemies(RoomNode room)
    {
        var poolConfigs = EnemyPool.Instance.poolConfigs;
        if (poolConfigs == null || poolConfigs.Count == 0) return;
        int totalTargetCount = 0;
        foreach (var config in poolConfigs) totalTargetCount += config.spawnAmount;
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
        else ForceClearRoom(room, runtimeData);
    }

    private void SpawnBoss(RoomNode room)
    {
        if (_isBossStarted) return;
        _isBossStarted = true;

        BossSpawner bossSpawner = FindAnyObjectByType<BossSpawner>();
        GameObject spawnedBossObj = null;

        if (bossSpawner != null)
            spawnedBossObj = bossSpawner.SpawnBoss();
        else return;

        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);
        if (runtimeData != null) runtimeData.StartCombat(1);

        if (spawnedBossObj != null && spawnedBossObj.TryGetComponent(out BossFSM bossFsm))
            bossFsm.SetRoom(room);
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
        GiveRewardToPlayer(gold, exp); 
        UpdateProgress(room);          
    }

    private void GiveRewardToPlayer(int gold, int exp)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (player.TryGetComponent(out IExperience expHandler))
            expHandler.AddExperience(exp);

        if (player.TryGetComponent(out PlayerController playerController))
            playerController.Gold += gold;
    }

    private void UpdateProgress(RoomNode room)
    {
        if (room == null) return;
        RoomRuntimeData runtimeData = _mapManager?.GetRuntimeData(room);
        if (runtimeData != null)
        {
            runtimeData.OnMonsterDead(); 
            if (runtimeData.state == RoomState.Cleared) 
                _mapManager?.OnCombatCleared(room); 
        }
    }
}

public class BossRoomDetector : MonoBehaviour
{
    private RoomNode _room;
    private RoomClearManager _manager;
    private bool _triggered = false;

    public void Setup(RoomNode room, RoomClearManager manager)
    {
        _room = room;
        _manager = manager;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_triggered && other.CompareTag("Player"))
        {
            _triggered = true;
            _manager.StartRoom(_room);
        }
    }
}