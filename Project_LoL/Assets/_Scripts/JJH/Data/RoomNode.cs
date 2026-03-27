using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    public string nodeId;
    public RoomData roomData;
    public List<RoomNode> neighbors = new List<RoomNode>();

    public Vector2Int size;
    public Vector2 worldPosition;
    
    [System.Obsolete("승열님의 RoomClearManager 호환용입니다.")]
    [Serializable]
    public class DoorData
    {
        // 기존에 승열님이 썼을 법한 최소한의 변수만 남겨둡니다.
        public Vector2Int position;
        public int direction;
    }
    
    [System.Obsolete("승열님의 EnemyPathfinder 호환용입니다.")]
    public Rect GetRect()
    {
        return new Rect(worldPosition.x, worldPosition.y, size.x, size.y);
    }

    // 문 데이터는 ConnectionResult에서 관리

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

    public RectInt GetBounds()
    {
        return new RectInt(
            Mathf.RoundToInt(worldPosition.x),
            Mathf.RoundToInt(worldPosition.y),
            size.x,
            size.y
        );
    }
}