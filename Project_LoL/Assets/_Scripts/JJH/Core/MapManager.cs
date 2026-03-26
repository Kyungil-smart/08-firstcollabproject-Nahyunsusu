using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("스테이지별 방 풀")]
    public MapRoomPool[] stagePools;
    
    [Header("맵 생성 설정")]
    [SerializeField] private int _roomSpacing = 15;

    [Header("타일맵 생성")]
    [SerializeField] private TileMapGenerator_Grid _tileMapGeneratorGrid;
    
    [Header("맵 출력")]
    public CorridorGenerator corridorGenerator;
    public DoorPlacer doorPlacer;

    [Header("문 시스템")]
    [SerializeField] private DoorController _doorController;

    private MapGraph _graph;
    private int _currentStageIndex;

    private Dictionary<RoomNode, RoomRuntimeData> _runtimeDataMap
        = new Dictionary<RoomNode, RoomRuntimeData>();

    public RoomNode CurrentRoom { get; private set; }

    public RoomRuntimeData CurrentRoomData => GetRuntimeData(CurrentRoom);

    private void Start()
    {
        BuildMap(0);
    }

    public void BuildMap(int stageIndex)
    {
        if (stageIndex >= stagePools.Length)
        {
            Debug.LogError($"스테이지 {stageIndex} 풀이 없습니다.");
            return;
        }

        _currentStageIndex = stageIndex;
        _graph = new MapGraph();
        _graph.Generate(stagePools[stageIndex], _roomSpacing);

        _runtimeDataMap.Clear();
        foreach (RoomNode room in _graph.allRooms)
            _runtimeDataMap[room] = new RoomRuntimeData(room);

        CurrentRoom = _graph.startRoom;

        if (doorPlacer != null)
            doorPlacer.PlaceDoors(_graph);
        else
            Debug.LogWarning("[MapManager] DoorPlacer 가 연결되어 있지 않습니다.");

        if (corridorGenerator != null)
            corridorGenerator.Generate(_graph);
        else
            Debug.LogWarning("[MapManager] CorridorGenerator 가 연결되어 있지 않습니다.");

        if (_tileMapGeneratorGrid != null && corridorGenerator != null)
            _tileMapGeneratorGrid.Generate(_graph, corridorGenerator.GetCorridors());
        else
            Debug.LogWarning("[MapManager] TileMapGenerator_Grid 또는 CorridorGenerator 가 연결되어 있지 않습니다.");

        // 문 생성
        if (_doorController != null)
            _doorController.BuildDoors(_graph);
        else
            Debug.LogWarning("[MapManager] DoorController 가 연결되어 있지 않습니다.");

        // RoomTrigger 생성
        CreateRoomTriggers();

        Debug.Log($"[MapManager] 스테이지 {stageIndex} 생성 완료 / 총 방 수: {_graph.allRooms.Count}");
        DebugPrintGraph();
    }

    private void CreateRoomTriggers()
    {
        foreach (RoomNode room in _graph.allRooms)
        {
            GameObject triggerObj = new GameObject($"RoomTrigger_{room.nodeId}");
            triggerObj.transform.SetParent(transform);
            triggerObj.transform.position = new Vector3(room.worldPosition.x, room.worldPosition.y, 0f);

            BoxCollider2D col = triggerObj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = room.size;

            RoomTrigger rt = triggerObj.AddComponent<RoomTrigger>();
            rt.room = room;
            rt.mapManager = this;
            rt.doorController = _doorController;
        }
    }

    public bool TryMoveToRoom(RoomNode next)
    {
        if (!CurrentRoom.neighbors.Contains(next))
        {
            Debug.LogWarning($"[MapManager] {next.nodeId} 는 현재 방과 연결되어 있지 않습니다.");
            return false;
        }
        
        RoomRuntimeData currentData = GetRuntimeData(CurrentRoom);

        if (CurrentRoom.roomData.roomType == RoomType.Combat &&
            currentData != null &&
            currentData.state != RoomState.Cleared)
        {
            Debug.LogWarning("[MapManager] 전투를 끝내야 이동할 수 있습니다.");
            return false;
        }
        
        CurrentRoom = next;

        Debug.Log($"[MapManager] → {next.nodeId} ({next.roomData.roomType})");
        return true;
    }

    public void OnCombatCleared(RoomNode room)
    {
        RoomRuntimeData data = GetRuntimeData(room);
        if (data == null)
            return;

        data.state = RoomState.Cleared;

        if (_doorController != null)
            _doorController.OpenDoors(room);
    }

    public void OnBossDefeated()
    {
        RoomRuntimeData bossData = GetRuntimeData(_graph.bossRoom);
        if (bossData != null)
            bossData.state = RoomState.Cleared;

        bool isLastStage = _currentStageIndex >= stagePools.Length - 1;

        if (isLastStage)
        {
            Debug.Log("[MapManager] 게임 클리어");
            // 실제 게임 클리어 처리는 GameManager 담당
        }
        else
        {
            Debug.Log("[MapManager] 보스 처치 / 포탈 생성");
            // 포탈 오브젝트 활성화 예정
        }
    }

    public RoomRuntimeData GetRuntimeData(RoomNode room)
    {
        if (room == null)
            return null;

        if (_runtimeDataMap.TryGetValue(room, out RoomRuntimeData data))
            return data;

        Debug.LogWarning($"[MapManager] {room.nodeId} 의 런타임 데이터가 없습니다.");
        return null;
    }

    private void DebugPrintGraph()
    {
        foreach (RoomNode room in _graph.allRooms)
        {
            string neighbors = string.Join(", ", room.neighbors.ConvertAll(n => n.nodeId));
            Debug.Log($"  [{room.nodeId} / {room.roomData.roomType}] " +
                      $"크기: {room.size.x}x{room.size.y} " +
                      $"위치: {room.worldPosition} " +
                      $"→ [{neighbors}]");
        }
    }
}