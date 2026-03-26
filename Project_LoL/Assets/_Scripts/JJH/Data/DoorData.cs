using UnityEngine;

public enum DoorDirection
{
    Up,
    Down,
    Left,
    Right
}

// 방에 배치되는 문 정보
public class DoorData
{
    public RoomNode connectedRoom;   // 이 문이 연결하는 이웃 방
    public DoorDirection direction;  // 문이 있는 벽면 방향
    public Vector2 localPosition;    // 방 중심 기준 문 위치 (벽면 위의 오프셋)

    public int openingWidth;         // 복도와 동일하게 사용할 문 폭

    public DoorData(RoomNode connectedRoom, DoorDirection direction, Vector2 localPosition)
    {
        this.connectedRoom = connectedRoom;
        this.direction = direction;
        this.localPosition = localPosition;

        this.openingWidth = 1; // 기본값 (복도 생성 시 덮어씀)
    }
}