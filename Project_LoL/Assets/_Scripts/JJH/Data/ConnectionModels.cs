using System.Collections.Generic;
using UnityEngine;
 
public enum DoorDir { Up, Down, Left, Right }
 
public class DoorCandidate
{
    public Vector2Int wallPos;      // 벽 위 좌표 (문이 생성될 타일)
    public Vector2Int entrance;     // 문 바깥 복도 시작 좌표 (wallPos + dir offset)
    public DoorDir dir;
    public RoomNode owner;
 
    public DoorCandidate(Vector2Int wall, DoorDir d, RoomNode o)
    {
        wallPos = wall;
        dir = d;
        owner = o;
        entrance = wall + DirOffset(d);
    }
 
    public static Vector2Int DirOffset(DoorDir d) => d switch
    {
        DoorDir.Up    => Vector2Int.up,
        DoorDir.Down  => Vector2Int.down,
        DoorDir.Left  => Vector2Int.left,
        _             => Vector2Int.right
    };
}
 
public class CorridorPath
{
    public List<Vector2Int> tiles;  // entrance ~ entrance 사이 복도 타일 목록
    public int bendCount;           // 꺾인 횟수 (0=직선, 1=L자, 2=2회 꺾기)
    public float score;
}
 
public class ConnectionResult
{
    public RoomNode roomA;
    public RoomNode roomB;
    public DoorCandidate doorA;
    public DoorCandidate doorB;
    public List<Vector2Int> corridorTiles;
}