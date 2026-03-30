using System.Collections.Generic;
using UnityEngine;

public class ConnectionPlanner : MonoBehaviour
{
    private HashSet<Vector2Int> _placedCorridors = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> _roomCells = new HashSet<Vector2Int>();
    private Dictionary<RoomNode, RectInt> _roomBounds = new Dictionary<RoomNode, RectInt>();

    public List<ConnectionResult> PlanAll(MapGraph graph)
    {
        _placedCorridors.Clear();
        _roomCells.Clear();
        _roomBounds.Clear();

        foreach (var room in graph.allRooms)
        {
            RectInt bounds = room.GetBounds();
            _roomBounds[room] = bounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            for (int y = bounds.yMin; y < bounds.yMax; y++)
                _roomCells.Add(new Vector2Int(x, y));
        }

        var results = new List<ConnectionResult>();
        var processed = new HashSet<string>();

        foreach (var room in graph.allRooms)
        {
            foreach (var neighbor in room.neighbors)
            {
                string key = EdgeKey(room, neighbor);
                if (!processed.Add(key)) continue;

                var result = FindBestConnection(room, neighbor);
                if (result == null) continue;

                results.Add(result);

                foreach (var t in result.corridorTiles)
                    _placedCorridors.Add(t);
            }
        }

        return results;
    }

    private ConnectionResult FindBestConnection(RoomNode a, RoomNode b)
    {
        if (IsBossPair(a, b))
            return ConnectVertical(a, b);

        var candidatesA = GetDoorCandidates(a);
        var candidatesB = GetDoorCandidates(b);

        ConnectionResult best = null;
        float bestScore = float.MaxValue;

        foreach (var ca in candidatesA)
        foreach (var cb in candidatesB)
        {
            var paths = BuildPaths(ca, cb);

            foreach (var path in paths)
            {
                if (IsBlocked(path.tiles, a, b)) continue;

                float score = Score(path, ca, cb);
                if (score >= bestScore) continue;

                bestScore = score;
                best = new ConnectionResult
                {
                    roomA = a,
                    roomB = b,
                    doorA = ca,
                    doorB = cb,
                    corridorTiles = path.tiles
                };
            }
        }

        return best;
    }

    private List<CorridorPath> BuildPaths(DoorCandidate ca, DoorCandidate cb)
    {
        Vector2Int s = ca.entrance;
        Vector2Int e = cb.entrance;

        var result = new List<CorridorPath>();

        // 직선
        if (s.x == e.x || s.y == e.y)
            AddPath(result, MakeLine(s, e), 0, ca, cb);

        // L자
        AddPath(result, ConcatLines(s, new Vector2Int(e.x, s.y), e), 1, ca, cb);
        AddPath(result, ConcatLines(s, new Vector2Int(s.x, e.y), e), 1, ca, cb);

        // 2번 꺾기
        int midX = (s.x + e.x) / 2;
        AddPath(result, ConcatLines(s,
            new Vector2Int(midX, s.y),
            new Vector2Int(midX, e.y),
            e), 2, ca, cb);

        int midY = (s.y + e.y) / 2;
        AddPath(result, ConcatLines(s,
            new Vector2Int(s.x, midY),
            new Vector2Int(e.x, midY),
            e), 2, ca, cb);

        return result;
    }

    private void AddPath(
        List<CorridorPath> list,
        List<Vector2Int> inner,
        int bends,
        DoorCandidate ca,
        DoorCandidate cb)
    {
        var tiles = new List<Vector2Int>();

        tiles.Add(ca.wallPos);

        for (int i = 0; i < inner.Count; i++)
        {
            Vector2Int pos = inner[i];

            if (pos == ca.wallPos) continue;
            if (pos == cb.wallPos) continue;

            tiles.Add(pos);
        }

        tiles.Add(cb.wallPos);

        list.Add(new CorridorPath
        {
            tiles = tiles,
            bendCount = bends
        });
    }

    private float Score(CorridorPath path, DoorCandidate ca, DoorCandidate cb)
    {
        float score = path.tiles.Count;
        score += path.bendCount * 20f;

        if (path.tiles.Count >= 3)
        {
            Vector2Int first = path.tiles[1] - path.tiles[0];
            if (first != DoorCandidate.DirOffset(ca.dir))
                score += 50f;

            Vector2Int last = path.tiles[path.tiles.Count - 1] - path.tiles[path.tiles.Count - 2];
            if (last != -DoorCandidate.DirOffset(cb.dir))
                score += 50f;
        }

        int overlapCount = 0;
        foreach (var t in path.tiles)
        {
            if (_placedCorridors.Contains(t))
                overlapCount++;
        }

        score += overlapCount * 5f;

        return score;
    }

    private bool IsBlocked(List<Vector2Int> tiles, RoomNode a, RoomNode b)
    {
        RectInt ba = _roomBounds[a];
        RectInt bb = _roomBounds[b];

        for (int i = 1; i < tiles.Count - 1; i++)
        {
            Vector2Int t = tiles[i];

            if (!_roomCells.Contains(t)) continue;

            bool inA = InBounds(ba, t);
            bool inB = InBounds(bb, t);

            if (!inA && !inB) return true;

            // 복도 중간이 자기 방 내부를 침범하면 차단
            if (inA || inB) return true;
        }

        return false;
    }

    private bool InBounds(RectInt r, Vector2Int p)
    {
        return p.x >= r.xMin && p.x < r.xMax &&
               p.y >= r.yMin && p.y < r.yMax;
    }

    private bool IsBossPair(RoomNode a, RoomNode b)
    {
        bool aBoss = a.roomData.roomType == RoomType.Boss;
        bool bBoss = b.roomData.roomType == RoomType.Boss;
        bool aRepair = a.roomData.roomType == RoomType.Repair;
        bool bRepair = b.roomData.roomType == RoomType.Repair;

        return (aBoss && bRepair) || (aRepair && bBoss);
    }

    private ConnectionResult ConnectVertical(RoomNode a, RoomNode b)
    {
        RoomNode lower = a.gridOrigin.y < b.gridOrigin.y ? a : b;
        RoomNode upper = lower == a ? b : a;

        RectInt lb = _roomBounds[lower];
        RectInt ub = _roomBounds[upper];

        int cx = lb.xMin + lb.width / 2;

        var wallLower = new Vector2Int(cx, lb.yMax - 1);
        var wallUpper = new Vector2Int(cx, ub.yMin);

        var ca = new DoorCandidate(wallLower, DoorDir.Up, lower);
        var cb = new DoorCandidate(wallUpper, DoorDir.Down, upper);

        var path = MakeLine(ca.entrance, cb.entrance);

        var list = new List<CorridorPath>();
        AddPath(list, path, 0, ca, cb);

        return new ConnectionResult
        {
            roomA = lower,
            roomB = upper,
            doorA = ca,
            doorB = cb,
            corridorTiles = list[0].tiles
        };
    }
    
    private List<DoorCandidate> GetDoorCandidates(RoomNode room)
    {
        var list = new List<DoorCandidate>();

        if (room.roomData.useFixedDoor)
        {
            Vector2Int wall = room.gridOrigin + room.roomData.fixedDoorLocalPosition;
            list.Add(new DoorCandidate(wall, (DoorDir)room.roomData.fixedDoorDirection, room));
            return list;
        }

        RectInt b = room.GetBounds();

        AddDoorCandidates(list, room, b, DoorDir.Up,
            new Vector2Int(b.xMin + b.width / 2, b.yMax - 1), Vector2Int.right);

        AddDoorCandidates(list, room, b, DoorDir.Down,
            new Vector2Int(b.xMin + b.width / 2, b.yMin), Vector2Int.right);

        AddDoorCandidates(list, room, b, DoorDir.Left,
            new Vector2Int(b.xMin, b.yMin + b.height / 2), Vector2Int.up);

        AddDoorCandidates(list, room, b, DoorDir.Right,
            new Vector2Int(b.xMax - 1, b.yMin + b.height / 2), Vector2Int.up);

        return list;
    }

    private void AddDoorCandidates(
        List<DoorCandidate> list, RoomNode room, RectInt b,
        DoorDir dir, Vector2Int center, Vector2Int axis)
    {
        for (int offset = -1; offset <= 1; offset++)
        {
            Vector2Int pos = center + axis * offset;
            if (!OnWall(pos, dir, b)) continue;
            list.Add(new DoorCandidate(pos, dir, room));
        }
    }

    private bool OnWall(Vector2Int pos, DoorDir dir, RectInt b)
    {
        if (dir == DoorDir.Up)    return pos.y == b.yMax - 1 && pos.x >= b.xMin && pos.x < b.xMax;
        if (dir == DoorDir.Down)  return pos.y == b.yMin     && pos.x >= b.xMin && pos.x < b.xMax;
        if (dir == DoorDir.Left)  return pos.x == b.xMin     && pos.y >= b.yMin && pos.y < b.yMax;
        if (dir == DoorDir.Right) return pos.x == b.xMax - 1 && pos.y >= b.yMin && pos.y < b.yMax;
        return false;
    }

    private List<Vector2Int> MakeLine(Vector2Int from, Vector2Int to)
    {
        var tiles = new List<Vector2Int>();

        int dx = from.x == to.x ? 0 : (to.x > from.x ? 1 : -1);
        int dy = from.y == to.y ? 0 : (to.y > from.y ? 1 : -1);

        Vector2Int cur = from;
        tiles.Add(cur);

        while (cur != to)
        {
            cur += new Vector2Int(dx, dy);
            tiles.Add(cur);
        }

        return tiles;
    }

    private List<Vector2Int> ConcatLines(params Vector2Int[] points)
    {
        var tiles = new List<Vector2Int>();

        for (int i = 0; i < points.Length - 1; i++)
        {
            var seg = MakeLine(points[i], points[i + 1]);

            if (tiles.Count > 0 && seg.Count > 0 && tiles[tiles.Count - 1] == seg[0])
                seg.RemoveAt(0);

            tiles.AddRange(seg);
        }

        return tiles;
    }

    private string EdgeKey(RoomNode a, RoomNode b)
    {
        if (string.Compare(a.nodeId, b.nodeId) < 0)
            return a.nodeId + "|" + b.nodeId;

        return b.nodeId + "|" + a.nodeId;
    }
}