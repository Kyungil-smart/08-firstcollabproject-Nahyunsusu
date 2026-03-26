using System.Collections.Generic;
using UnityEngine;

public enum RoomState
{
    Unvisited,   // 미입장
    InProgress,  // 진행 중
    Cleared      // 클리어
}

public class RoomNode
{
    public string nodeId;          // 디버그용 ID
    public RoomData roomData;      // 실제 방 데이터 (타입, 설정 등)
    public RoomState state;        // 현재 진행 상태
    public List<RoomNode> neighbors; // 연결된 인접 노드 (양방향 그래프)
    
    public Vector2Int size;        // 방 크기 (타일 단위)
    public Vector2 worldPosition;  // 씬 상 위치

    // 방에 연결된 문 정보
    public List<DoorData> doors;
    
    public RoomNode(string id, RoomData data)
    {
        nodeId = id;
        roomData = data;
        state = RoomState.Unvisited;
        neighbors = new List<RoomNode>();
        doors = new List<DoorData>();
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
    
    public Rect GetRect()
    {
        // worldPosition이 중심점일 경우
        return new Rect(
            worldPosition.x - (size.x * 0.5f),
            worldPosition.y - (size.y * 0.5f),
            size.x,
            size.y
        );
    }
}