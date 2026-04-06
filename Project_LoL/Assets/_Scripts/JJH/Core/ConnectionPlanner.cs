using System.Collections.Generic;
using UnityEngine;
 
public class ConnectionPlanner : MonoBehaviour
{
    [SerializeField] private int _bendClearance = 10;
    [SerializeField] private int _roomClearance = 5;
    [SerializeField] private int _bendStepCount = 5;
    [SerializeField] private int _corridorWidthMin = 2;
    [SerializeField] private int _corridorWidthMax = 6;
    [SerializeField] private int _bossCorridorWidth = 5;
 
    private HashSet<Vector2Int> _roomCells = new HashSet<Vector2Int>();
    private Dictionary<RoomNode, RectInt> _roomBounds = new Dictionary<RoomNode, RectInt>();
 
    public List<ConnectionResult> PlanAll(MapGraph graph)
    {
        _roomCells.Clear();
        _roomBounds.Clear();
 
        foreach (var room in graph.allRooms)
        {
            RectInt b = room.GetBounds();
            _roomBounds[room] = b;
 
            for (int x = b.xMin; x < b.xMax; x++)
                for (int y = b.yMin; y < b.yMax; y++)
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
 
                var result = FindConnection(room, neighbor);
                if (result == null) continue;
 
                results.Add(result);
            }
        }
 
        return results;
    }
 
    private ConnectionResult FindConnection(RoomNode a, RoomNode b)
    {
        if (IsBossPair(a, b))
            return ConnectVertical(a, b);

        int width = Random.Range(_corridorWidthMin, _corridorWidthMax + 1);

        List<DoorCandidate> candidatesA = GetDoorCandidates(a, b);
        List<DoorCandidate> candidatesB = GetDoorCandidates(b, a);

        foreach (var ca in candidatesA)
        {
            foreach (var cb in candidatesB)
            {
                bool isVertical = ca.dir == DoorDir.Up || ca.dir == DoorDir.Down;
                var result = TryConnect(a, b, ca, cb, width, isVertical);
                if (result != null)
                    return result;
            }
        }

        return null;
    }
 
    private List<DoorCandidate> GetDoorCandidates(RoomNode room, RoomNode other)
    {
        var list = new List<DoorCandidate>();
 
        if (room.roomData.useFixedDoor)
        {
            Vector2Int wall = room.gridOrigin + room.roomData.fixedDoorLocalPosition;
            list.Add(new DoorCandidate(wall, (DoorDir)room.roomData.fixedDoorDirection, room));
            return list;
        }
 
        RectInt b = room.GetBounds();
        RectInt ob = _roomBounds[other];
 
        Vector2Int roomCenter = new Vector2Int(b.xMin + b.width / 2, b.yMin + b.height / 2);
        Vector2Int otherCenter = new Vector2Int(ob.xMin + ob.width / 2, ob.yMin + ob.height / 2);
 
        int dx = otherCenter.x - roomCenter.x;
        int dy = otherCenter.y - roomCenter.y;
 
        bool preferHorizontal = Mathf.Abs(dx) >= Mathf.Abs(dy);
 
        if (preferHorizontal)
        {
            if (dx > 0)
                AddDoorCandidates(list, room, b, DoorDir.Right,
                    new Vector2Int(b.xMax, b.yMin + b.height / 2), Vector2Int.up);
            else
                AddDoorCandidates(list, room, b, DoorDir.Left,
                    new Vector2Int(b.xMin - 1, b.yMin + b.height / 2), Vector2Int.up);
        }
        else
        {
            if (dy > 0)
                AddDoorCandidates(list, room, b, DoorDir.Up,
                    new Vector2Int(b.xMin + b.width / 2, b.yMax), Vector2Int.right);
            else
                AddDoorCandidates(list, room, b, DoorDir.Down,
                    new Vector2Int(b.xMin + b.width / 2, b.yMin - 1), Vector2Int.right);
        }
 
        return list;
    }
 
    private ConnectionResult TryConnect(
        RoomNode a, RoomNode b,
        DoorCandidate ca, DoorCandidate cb,
        int width, bool isVertical)
    {
        var straight = TryStraight(ca, cb, a, b, width, isVertical);
        if (straight != null)
            return MakeResult(a, b, ca, cb, straight, width);
 
        var lShape = TryLShape(ca, cb, a, b, width, isVertical);
        if (lShape != null)
            return MakeResult(a, b, ca, cb, lShape, width);
 
        var doubleBend = TryDoubleBend(ca, cb, a, b, width, isVertical);
        if (doubleBend != null)
            return MakeResult(a, b, ca, cb, doubleBend, width);
 
        return null;
    }
 
    private List<Vector2Int> TryStraight(
        DoorCandidate ca, DoorCandidate cb,
        RoomNode a, RoomNode b,
        int width, bool isVertical)
    {
        Vector2Int s = ca.entrance;
        Vector2Int e = cb.entrance;
 
        if (s.x != e.x && s.y != e.y)
            return null;
 
        var tiles = MakeLine(s, e);
        if (!PathClear(tiles, a, b, width, isVertical))
            return null;
 
        return Wrap(tiles, ca.tileWallPos, cb.tileWallPos);
    }
 
    private List<Vector2Int> TryLShape(
        DoorCandidate ca, DoorCandidate cb,
        RoomNode a, RoomNode b,
        int width, bool isVertical)
    {
        Vector2Int s = ca.entrance;
        Vector2Int e = cb.entrance;
 
        Vector2Int[] corners = {
            new Vector2Int(e.x, s.y),
            new Vector2Int(s.x, e.y)
        };
 
        foreach (var corner in corners)
        {
            if (!BendClear(corner, ca)) continue;
            if (!BendClear(corner, cb)) continue;
 
            var tiles = ConcatLines(s, corner, e);
            if (!PathClear(tiles, a, b, width, isVertical)) continue;
 
            return Wrap(tiles, ca.tileWallPos, cb.tileWallPos);
        }
 
        return null;
    }
 
    private List<Vector2Int> TryDoubleBend(
        DoorCandidate ca, DoorCandidate cb,
        RoomNode a, RoomNode b,
        int width, bool isVertical)
    {
        Vector2Int s = ca.entrance;
        Vector2Int e = cb.entrance;
 
        int xMin = Mathf.Min(s.x, e.x);
        int xMax = Mathf.Max(s.x, e.x);
        int yMin = Mathf.Min(s.y, e.y);
        int yMax = Mathf.Max(s.y, e.y);
 
        int xStep = Mathf.Max(1, (xMax - xMin) / _bendStepCount);
        int yStep = Mathf.Max(1, (yMax - yMin) / _bendStepCount);
 
        for (int midX = xMin; midX <= xMax; midX += xStep)
        {
            var p1 = new Vector2Int(midX, s.y);
            var p2 = new Vector2Int(midX, e.y);
 
            if (!BendClear(p1, ca)) continue;
            if (!BendClear(p2, cb)) continue;
 
            var tiles = ConcatLines(s, p1, p2, e);
            if (!PathClear(tiles, a, b, width, isVertical)) continue;
 
            return Wrap(tiles, ca.tileWallPos, cb.tileWallPos);
        }
 
        for (int midY = yMin; midY <= yMax; midY += yStep)
        {
            var p1 = new Vector2Int(s.x, midY);
            var p2 = new Vector2Int(e.x, midY);
 
            if (!BendClear(p1, ca)) continue;
            if (!BendClear(p2, cb)) continue;
 
            var tiles = ConcatLines(s, p1, p2, e);
            if (!PathClear(tiles, a, b, width, isVertical)) continue;
 
            return Wrap(tiles, ca.tileWallPos, cb.tileWallPos);
        }
 
        return null;
    }
 
    private bool BendClear(Vector2Int bend, DoorCandidate door)
    {
        Vector2Int offset = DoorCandidate.DirOffset(door.dir);
 
        if (offset.x != 0)
            return Mathf.Abs(bend.x - door.entrance.x) >= _bendClearance;
        else
            return Mathf.Abs(bend.y - door.entrance.y) >= _bendClearance;
    }
 
    private bool PathClear(
        List<Vector2Int> tiles, RoomNode a, RoomNode b,
        int width, bool isVertical)
    {
        int half = width / 2;
 
        foreach (var t in tiles)
        {
            for (int i = -half; i < width - half; i++)
            {
                Vector2Int expanded = isVertical
                    ? new Vector2Int(t.x + i, t.y)
                    : new Vector2Int(t.x, t.y + i);
 
                foreach (var pair in _roomBounds)
                {
                    if (pair.Key == a || pair.Key == b) continue;
 
                    RectInt rb = pair.Value;
                    int distX = 0;
                    int distY = 0;
 
                    if (expanded.x < rb.xMin) distX = rb.xMin - expanded.x;
                    else if (expanded.x >= rb.xMax) distX = expanded.x - (rb.xMax - 1);
 
                    if (expanded.y < rb.yMin) distY = rb.yMin - expanded.y;
                    else if (expanded.y >= rb.yMax) distY = expanded.y - (rb.yMax - 1);
 
                    if (distX + distY < _roomClearance)
                        return false;
                }
            }
        }
 
        return true;
    }
 
    private List<Vector2Int> Wrap(List<Vector2Int> inner, Vector2Int wallA, Vector2Int wallB)
    {
        var result = new List<Vector2Int>();
        result.Add(wallA);
        foreach (var t in inner)
        {
            if (t != wallA && t != wallB)
                result.Add(t);
        }
        result.Add(wallB);
        return result;
    }
 
    private ConnectionResult MakeResult(
        RoomNode a, RoomNode b,
        DoorCandidate ca, DoorCandidate cb,
        List<Vector2Int> tiles,
        int width)
    {
        return new ConnectionResult
        {
            roomA = a,
            roomB = b,
            doorA = ca,
            doorB = cb,
            corridorTiles = tiles,
            corridorWidth = width
        };
    }
 
    private bool IsBossPair(RoomNode a, RoomNode b)
    {
        bool aBoss = a.roomData.roomType == RoomType.Boss;
        bool bBoss = b.roomData.roomType == RoomType.Boss;
        bool aCombat = a.roomData.roomType == RoomType.Combat;
        bool bCombat = b.roomData.roomType == RoomType.Combat;
        return (aBoss && bCombat) || (aCombat && bBoss);
    }
 
    private ConnectionResult ConnectVertical(RoomNode a, RoomNode b)
    {
        RoomNode lower = a.gridOrigin.y < b.gridOrigin.y ? a : b;
        RoomNode upper = lower == a ? b : a;

        RectInt lb = _roomBounds[lower];
        RectInt ub = _roomBounds[upper];

        int upperCx = ub.xMin + ub.width / 2 - 1;
        int cx = Mathf.Clamp(upperCx, lb.xMin, lb.xMax - 1);

        var wallLower = new Vector2Int(cx, lb.yMax);
        var wallUpper = new Vector2Int(cx, ub.yMin - 1);

        var ca = new DoorCandidate(wallLower, DoorDir.Up, lower);
        var cb = new DoorCandidate(wallUpper, DoorDir.Down, upper);

        var corridor = Wrap(MakeLine(ca.entrance, cb.entrance), ca.tileWallPos, cb.tileWallPos);

        return new ConnectionResult
        {
            roomA = lower,
            roomB = upper,
            doorA = ca,
            doorB = cb,
            corridorTiles = corridor,
            corridorWidth = _bossCorridorWidth
        };
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
        if (dir == DoorDir.Up)    return pos.y == b.yMax     && pos.x >= b.xMin && pos.x < b.xMax;
        if (dir == DoorDir.Down)  return pos.y == b.yMin - 1 && pos.x >= b.xMin && pos.x < b.xMax;
        if (dir == DoorDir.Left)  return pos.x == b.xMin - 1 && pos.y >= b.yMin && pos.y < b.yMax;
        if (dir == DoorDir.Right) return pos.x == b.xMax     && pos.y >= b.yMin && pos.y < b.yMax;
        return false;
    }
 
    private List<Vector2Int> MakeLine(Vector2Int from, Vector2Int to)
    {
        var tiles = new List<Vector2Int>();
        int dx = 0;
        int dy = 0;
        if (to.x != from.x) dx = to.x > from.x ? 1 : -1;
        if (to.y != from.y) dy = to.y > from.y ? 1 : -1;
 
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