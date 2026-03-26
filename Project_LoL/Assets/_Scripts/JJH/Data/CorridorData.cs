using UnityEngine;

// 두 방을 연결하는 복도 데이터
public class CorridorData
{
    public RoomNode roomA;        // 연결 시작 방
    public RoomNode roomB;        // 연결 끝 방
    public float width;           // 복도 폭 (2~6 랜덤)
    public Vector2[] points;      // 복도 꺾임 경로 (L자 또는 직선)

    public CorridorData(RoomNode a, RoomNode b, float width, Vector2[] points)
    {
        roomA = a;
        roomB = b;
        this.width = width;
        this.points = points;
    }
}