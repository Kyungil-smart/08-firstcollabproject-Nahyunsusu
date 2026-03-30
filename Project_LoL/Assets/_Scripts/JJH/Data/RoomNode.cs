using System.Collections.Generic;
using UnityEngine;
 
public class RoomNode
{
    public string nodeId;
    public RoomData roomData;
    public List<RoomNode> neighbors = new List<RoomNode>();
 
    public Vector2Int gridOrigin;   // grid 기준 좌하단 좌표
    public Vector2Int size;
 
    public RoomNode(string id, RoomData data)
    {
        nodeId = id;
        roomData = data;
    }
 
    public void ConnectTo(RoomNode other)
    {
        if (other == null || other == this) return;
        if (!neighbors.Contains(other)) neighbors.Add(other);
        if (!other.neighbors.Contains(this)) other.neighbors.Add(this);
    }
 
    public RectInt GetBounds() => new RectInt(gridOrigin, size);
}