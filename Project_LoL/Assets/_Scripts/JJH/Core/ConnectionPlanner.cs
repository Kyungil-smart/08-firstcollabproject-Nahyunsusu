using System.Collections.Generic;
using System.Linq;
using UnityEngine;
 
public class ConnectionPlanner : MonoBehaviour
{
    private HashSet<Vector2Int> _placedCorridors = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> _roomCells = new HashSet<Vector2Int>();
    
    public List<ConnectionResult> PlanAll(MapGraph graph)
    {
        _placedCorridors.Clear();
        _roomCells.Clear();
 
        foreach (var room in graph.allRooms)
        {
            RectInt b = room.GetBounds();
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
        List<DoorCandidate> candidatesA = GetDoorCandidates(a);
        List<DoorCandidate> candidatesB = GetDoorCandidates(b);
 
        ConnectionResult best = null;
        float bestScore = float.MaxValue;
 
        foreach (var ca in candidatesA)
        {
            foreach (var cb in candidatesB)
            {
                if (!CanDirectionallyConnect(ca.dir, cb.dir)) continue;
 
                List<CorridorPath> paths = BuildCandidatePaths(ca.entrance, cb.entrance, ca.dir, cb.dir);
 
                foreach (var path in paths)
                {
                    if (IsBlocked(path.tiles, a, b)) continue;
 
                    float score = ScorePath(path, ca, cb);
                    if (score < bestScore)
                    {
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
            }
        }
 
        return best;
    }
 
    private List<CorridorPath> BuildCandidatePaths(
        Vector2Int start, Vector2Int end, DoorDir dirA, DoorDir dirB)
    {
        var paths = new List<CorridorPath>();
        
        if (start.x == end.x || start.y == end.y)
        {
            var straight = MakeStraightLine(start, end);
            if (straight != null) paths.Add(new CorridorPath { tiles = straight, bendCount = 0, score = 0 });
        }
        {
            var corner = new Vector2Int(end.x, start.y);
            var xy = ConcatLines(start, corner, end);
            paths.Add(new CorridorPath { tiles = xy, bendCount = 1, score = 0 });
        }
        {
            var corner = new Vector2Int(start.x, end.y);
            var yx = ConcatLines(start, corner, end);
            paths.Add(new CorridorPath { tiles = yx, bendCount = 1, score = 0 });
        }
        {
            int midX = (start.x + end.x) / 2;
            var p1 = new Vector2Int(midX, start.y);
            var p2 = new Vector2Int(midX, end.y);
            var path2a = ConcatLines(start, p1, p2, end);
            paths.Add(new CorridorPath { tiles = path2a, bendCount = 2, score = 0 });
        }
        {
            int midY = (start.y + end.y) / 2;
            var p1 = new Vector2Int(start.x, midY);
            var p2 = new Vector2Int(end.x, midY);
            var path2b = ConcatLines(start, p1, p2, end);
            paths.Add(new CorridorPath { tiles = path2b, bendCount = 2, score = 0 });
        }
 
        return paths;
    }
    
    private float ScorePath(CorridorPath path, DoorCandidate ca, DoorCandidate cb)
    {
        float score = path.tiles.Count;
        
        score += path.bendCount * 20f;
        
        if (path.tiles.Count >= 2)
        {
            Vector2Int firstStep = path.tiles[1] - path.tiles[0];
            if (firstStep != DoorCandidate.DirOffset(ca.dir))
                score += 50f;
 
            Vector2Int lastStep = path.tiles[path.tiles.Count - 1] - path.tiles[path.tiles.Count - 2];
            
            if (lastStep != -DoorCandidate.DirOffset(cb.dir))
                score += 50f;
        }
        
        foreach (var t in path.tiles)
            if (_placedCorridors.Contains(t)) score += 5f;
 
        return score;
    }
    
    private bool IsBlocked(List<Vector2Int> tiles, RoomNode a, RoomNode b)
    {
        RectInt ba = a.GetBounds();
        RectInt bb = b.GetBounds();
 
        foreach (var t in tiles)
        {
            if (!_roomCells.Contains(t)) continue;
            
            bool inA = ba.Contains(t);
            bool inB = bb.Contains(t);
            if (!inA && !inB) return true;
        }
        return false;
    }
    
    private bool CanDirectionallyConnect(DoorDir a, DoorDir b)
    {
        if (a == b) return false;
        return true;
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
        
        AddWallCandidates(list, room, b, DoorDir.Up,
            new Vector2Int(b.xMin + b.width / 2, b.yMax - 1),
            Vector2Int.right);
        
        AddWallCandidates(list, room, b, DoorDir.Down,
            new Vector2Int(b.xMin + b.width / 2, b.yMin),
            Vector2Int.right);
        
        AddWallCandidates(list, room, b, DoorDir.Left,
            new Vector2Int(b.xMin, b.yMin + b.height / 2),
            Vector2Int.up);
        
        AddWallCandidates(list, room, b, DoorDir.Right,
            new Vector2Int(b.xMax - 1, b.yMin + b.height / 2),
            Vector2Int.up);
 
        return list;
    }
    
    private void AddWallCandidates(
        List<DoorCandidate> list,
        RoomNode room,
        RectInt b,
        DoorDir dir,
        Vector2Int center,
        Vector2Int sideAxis)
    {
        for (int offset = -1; offset <= 1; offset++)
        {
            Vector2Int pos = center + sideAxis * offset;
            if (!IsOnWall(pos, dir, b)) continue;
            list.Add(new DoorCandidate(pos, dir, room));
        }
    }
    
    private bool IsOnWall(Vector2Int pos, DoorDir dir, RectInt b)
    {
        switch (dir)
        {
            case DoorDir.Up:
                return pos.y == b.yMax - 1 && pos.x >= b.xMin && pos.x < b.xMax;
            case DoorDir.Down:
                return pos.y == b.yMin && pos.x >= b.xMin && pos.x < b.xMax;
            case DoorDir.Left:
                return pos.x == b.xMin && pos.y >= b.yMin && pos.y < b.yMax;
            case DoorDir.Right:
                return pos.x == b.xMax - 1 && pos.y >= b.yMin && pos.y < b.yMax;
        }
        return false;
    }
 
    private List<Vector2Int> MakeStraightLine(Vector2Int from, Vector2Int to)
    {
        var tiles = new List<Vector2Int>();
        Vector2Int cur = from;
        Vector2Int step = new Vector2Int(
            to.x != from.x ? (int)Mathf.Sign(to.x - from.x) : 0,
            to.y != from.y ? (int)Mathf.Sign(to.y - from.y) : 0
        );
        tiles.Add(cur);
        while (cur != to)
        {
            cur += step;
            tiles.Add(cur);
        }
        return tiles;
    }
    
    private List<Vector2Int> ConcatLines(params Vector2Int[] points)
    {
        var tiles = new List<Vector2Int>();
        for (int i = 0; i < points.Length - 1; i++)
        {
            var seg = MakeStraightLine(points[i], points[i + 1]);
            // 이전 끝점 중복 제거
            if (tiles.Count > 0 && seg.Count > 0 && tiles[tiles.Count - 1] == seg[0])
                seg.RemoveAt(0);
            tiles.AddRange(seg);
        }
        return tiles;
    }
 
    private string EdgeKey(RoomNode a, RoomNode b) =>
        string.Compare(a.nodeId, b.nodeId) < 0
            ? $"{a.nodeId}|{b.nodeId}"
            : $"{b.nodeId}|{a.nodeId}";
}