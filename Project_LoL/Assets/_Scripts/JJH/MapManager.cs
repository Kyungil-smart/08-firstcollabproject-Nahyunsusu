using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("스테이지별 방 풀")]
    public MapRoomPool[] stagePools;

    private MapGraph _graph;
    private int _currentStageIndex;
    public RoomNode CurrentRoom { get; private set; }

    void Start()
    {
        BuildMap(0);
    }

    // 스테이지 인덱스에 맞는 맵 그래프 생성
    public void BuildMap(int stageIndex)
    {
        if (stageIndex >= stagePools.Length)
        {
            Debug.LogError($"스테이지 {stageIndex} 풀이 없습니다.");
            return;
        }

        _currentStageIndex = stageIndex;
        _graph = new MapGraph();
        _graph.Generate(stagePools[stageIndex]);

        CurrentRoom = _graph.startRoom;
        CurrentRoom.isVisited = true;

        Debug.Log($"[MapManager] 스테이지 {stageIndex} 생성 완료 / 총 방 수: {_graph.allRooms.Count}");
        DebugPrintGraph();
    }

    // 현재 방과 연결된 다음 방으로 이동 시도
    public bool TryMoveToRoom(RoomNode next)
    {
        if (!CurrentRoom.neighbors.Contains(next))
        {
            Debug.LogWarning($"[MapManager] {next.nodeId} 는 현재 방과 연결되어 있지 않습니다.");
            return false;
        }

        // 전투 방은 클리어 전 이동 불가
        if (CurrentRoom.roomData.roomType == RoomType.Combat && !CurrentRoom.isCleared)
        {
            Debug.LogWarning("[MapManager] 전투를 끝내야 이동할 수 있습니다.");
            return false;
        }

        CurrentRoom = next;
        CurrentRoom.isVisited = true;

        Debug.Log($"[MapManager] → {next.nodeId} ({next.roomData.roomType})");
        return true;
    }

    // 보스 처치 후 현재 스테이지 종료 처리
    public void OnBossDefeated()
    {
        _graph.bossRoom.isCleared = true;

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
            Debug.Log($"  [{room.nodeId} / {room.roomData.roomType}] → [{neighbors}]");
        }
    }
}