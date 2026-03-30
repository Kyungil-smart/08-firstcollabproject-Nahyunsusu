using System.Collections.Generic;
using UnityEngine;
 
public enum DoorDir { Up, Down, Left, Right }
 
public class DoorCandidate
{
    public Vector2Int wallPos;
    public Vector2Int entrance;
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