using System.Collections.Generic;
using UnityEngine;

public enum DoorDir { Up, Down, Left, Right }

public class DoorCandidate
{
    public Vector2Int wallPos;      // 문 오브젝트 스폰 pivot (RecalcWallPos 후)
    public Vector2Int tileWallPos;  // 타일맵 벽 뚫기용 — 복도 시작(-X/-Y) 벽, 원본값 유지
    public Vector2Int entrance;
    public DoorDir dir;
    public RoomNode owner;

    public DoorCandidate(Vector2Int wall, DoorDir d, RoomNode o)
    {
        wallPos = wall;
        tileWallPos = wall;  // 원본 보존
        dir = d;
        owner = o;
        entrance = wall + DirOffset(d);
    }
    
    public void RecalcWallPos(int width)
    {
        int half = width / 2;

        switch (dir)
        {
            case DoorDir.Up:
                wallPos = new Vector2Int(tileWallPos.x + (width - half - 1), tileWallPos.y);
                break;
            case DoorDir.Down:
                wallPos = new Vector2Int(tileWallPos.x - half, tileWallPos.y);
                break;
            case DoorDir.Right:
                wallPos = new Vector2Int(tileWallPos.x, tileWallPos.y - half);
                break;
            case DoorDir.Left:
                wallPos = new Vector2Int(tileWallPos.x, tileWallPos.y + (width - half - 1));
                break;
        }
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
    public List<Vector2Int> tiles;
    public int bendCount;
    public float score;
}

public class ConnectionResult
{
    public RoomNode roomA;
    public RoomNode roomB;
    public DoorCandidate doorA;
    public DoorCandidate doorB;
    public List<Vector2Int> corridorTiles;
    public int corridorWidth;
}