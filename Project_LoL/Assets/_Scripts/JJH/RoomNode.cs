using System.Collections.Generic;

// 그래프 기반 맵에서 사용하는 노드
// 각 방(RoomData)을 하나의 노드로 보고 연결 구조를 관리
public class RoomNode
{
    public string nodeId;          // 디버그 / 식별용 ID (로그, 추적용)
    
    public RoomData roomData;      // 실제 방 데이터 (타입, 설정 등)
    
    public bool isVisited;         // 플레이어가 한 번이라도 진입했는지
    
    public bool isCleared;         // 해당 방의 이벤트(전투 등) 완료 여부
    
    public List<RoomNode> neighbors; // 연결된 인접 노드 (양방향 그래프)

    public RoomNode(string id, RoomData data)
    {
        nodeId = id;
        roomData = data;

        isVisited = false;
        isCleared = false;

        neighbors = new List<RoomNode>();
    }

    // 다른 노드와 연결 (양방향)
    // 그래프 구조이기 때문에 서로 참조하도록 구성
    public void ConnectTo(RoomNode other)
    {
        if (!neighbors.Contains(other))
            neighbors.Add(other);

        if (!other.neighbors.Contains(this))
            other.neighbors.Add(this);
    }
}