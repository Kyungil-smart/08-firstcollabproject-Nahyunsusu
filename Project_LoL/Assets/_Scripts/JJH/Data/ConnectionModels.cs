using System.Collections.Generic;
using UnityEngine;

public enum DoorDir
{
    Up,
    Down,
    Left,
    Right
}

public class DoorCandidate
{
    public Vector2Int pos;          // 문 타일 좌표
    public Vector2Int entrance;     // 문 바깥쪽 복도 시작 좌표
    public DoorDir dir;
    public RoomNode owner;

    public DoorCandidate(Vector2Int p, DoorDir d, RoomNode o)
    {
        pos = p;
        dir = d;
        owner = o;
        entrance = p + GetOffset(d);
    }

    public static Vector2Int GetOffset(DoorDir d)
    {
        return d switch
        {
            DoorDir.Up => Vector2Int.up,
            DoorDir.Down => Vector2Int.down,
            DoorDir.Left => Vector2Int.left,
            _ => Vector2Int.right
        };
    }
}

public class ConnectionResult
{
    public RoomNode roomA;
    public RoomNode roomB;
    public DoorCandidate doorA;
    public DoorCandidate doorB;
    public List<Vector2Int> corridorPoints;   // 복도 타일 좌표
}