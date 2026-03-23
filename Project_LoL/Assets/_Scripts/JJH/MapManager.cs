// MapManager.cs
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
        _graph.Generate(stagePools[stageIndex]);

        CurrentRoom = _graph.startRoom;
        CurrentRoom.state = RoomState.InProgress;

        Debug.Log($"[MapManager] 스테이지 {stageIndex} 생성 완료 / 총 방 수: {_graph.allRooms.Count}");
        DebugPrintGraph();
    }

    // 이동 가능 여부를 검사하고 다음 방으로 전환
    // 방 클리어 상태는 외부 시스템에서 관리
    public bool TryMoveToRoom(RoomNode next)
    {
        if (!CurrentRoom.neighbors.Contains(next))
        {
            Debug.LogWarning($"[MapManager] {next.nodeId} 는 현재 방과 연결되어 있지 않습니다.");
            return false;
        }

        if (CurrentRoom.roomData.roomType == RoomType.Combat &&
            CurrentRoom.state != RoomState.Cleared)
        {
            Debug.LogWarning("[MapManager] 전투를 끝내야 이동할 수 있습니다.");
            return false;
        }

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
            // GameManager 담당
        }
        else
        {
            Debug.Log("[MapManager] 보스 처치 / 포탈 생성");
            // 포탈 오브젝트 활성화는 나중에
        }
    }

    private void DebugPrintGraph()
    {
        foreach (var room in _graph.allRooms)
        {
            var neighbors = string.Join(", ", room.neighbors.ConvertAll(n => n.nodeId));
            Debug.Log($"  [{room.nodeId} / {room.roomData.roomType}] → [{neighbors}]");
        }
    }
}