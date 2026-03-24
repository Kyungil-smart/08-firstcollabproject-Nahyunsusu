using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("스테이지별 방 풀")]
    public MapRoomPool[] stagePools;
    
    [Header("맵 생성 설정")]
    [SerializeField] private int roomSpacing = 15;
    
    [Header("맵 출력")]
    public CorridorGenerator corridorGenerator;
    public RoomVisualizer roomVisualizer;
    public DoorPlacer doorPlacer;
    public DoorVisualizer doorVisualizer;

    private MapGraph _graph;
    private int _currentStageIndex;
    public RoomNode CurrentRoom { get; private set; }

    void Start()
    {
        BuildMap(0);
    }

    // MapGraph 생성 및 현재 스테이지 초기화
    public void BuildMap(int stageIndex)
    {
        if (stageIndex >= stagePools.Length)
        {
            Debug.LogError($"스테이지 {stageIndex} 풀이 없습니다.");
            return;
        }

        _currentStageIndex = stageIndex;
        _graph = new MapGraph();
        _graph.Generate(stagePools[stageIndex], roomSpacing);

        CurrentRoom = _graph.startRoom;
        CurrentRoom.state = RoomState.InProgress;
        
        // 맵 시각화 및 연결 요소 생성
        if (corridorGenerator != null)
            corridorGenerator.Generate(_graph);
        else
            Debug.LogWarning("[MapManager] CorridorGenerator 가 연결되어 있지 않습니다.");
        
        if (roomVisualizer != null)
            roomVisualizer.Visualize(_graph);
        else
            Debug.LogWarning("[MapManager] RoomVisualizer 가 연결되어 있지 않습니다.");

        if (doorPlacer != null)
            doorPlacer.PlaceDoors(_graph);
        else
            Debug.LogWarning("[MapManager] DoorPlacer 가 연결되어 있지 않습니다.");

        if (doorVisualizer != null)
            doorVisualizer.Visualize(_graph);
        else
            Debug.LogWarning("[MapManager] DoorVisualizer 가 연결되어 있지 않습니다.");

        Debug.Log($"[MapManager] 스테이지 {stageIndex} 생성 완료 / 총 방 수: {_graph.allRooms.Count}");
        DebugPrintGraph();
    }

    // 이동 가능 여부를 검사하고 다음 방으로 전환
    // 방 클리어 상태는 외부 시스템에서 관리
    public bool TryMoveToRoom(RoomNode next)
    {
        // 연결된 방인지 확인
        if (!CurrentRoom.neighbors.Contains(next))
        {
            Debug.LogWarning($"[MapManager] {next.nodeId} 는 현재 방과 연결되어 있지 않습니다.");
            return false;
        }
        
        // 연결된 방인지 확인
        if (CurrentRoom.roomData.roomType == RoomType.Combat &&
            CurrentRoom.state != RoomState.Cleared)
        {
            Debug.LogWarning("[MapManager] 전투를 끝내야 이동할 수 있습니다.");
            return false;
        }
        
        // 이동 처리 (상태 변경은 외부 시스템에서 처리)
        CurrentRoom.state = RoomState.Cleared;
        CurrentRoom = next;
        CurrentRoom.state = RoomState.InProgress;

        Debug.Log($"[MapManager] → {next.nodeId} ({next.roomData.roomType})");
        return true;
    }

    // 보스 처치 후 스테이지 종료 처리
    public void OnBossDefeated()
    {
        _graph.bossRoom.state = RoomState.Cleared;

        bool isLastStage = _currentStageIndex >= stagePools.Length - 1;

        if (isLastStage)
        {
            Debug.Log("[MapManager] 게임 클리어");
            // 실제 게임 클리어 처리는 GameManager 담당
        }
        else
        {
            Debug.Log("[MapManager] 보스 처치 / 포탈 생성");
            // 다음 스테이지 이동용 포탈 활성화 예정
        }
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