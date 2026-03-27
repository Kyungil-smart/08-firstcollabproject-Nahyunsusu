using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConnectionPlanner : MonoBehaviour
{
    private HashSet<Vector2Int> _roomOccupiedCells = new HashSet<Vector2Int>();

    public List<ConnectionResult> PlanAll(MapGraph graph)
    {
        _roomOccupiedCells.Clear();
        var results = new List<ConnectionResult>();

        // 방 점유 영역 등록
        foreach (var room in graph.allRooms)
        {
            RectInt bounds = GetRoomBounds(room);
            for (int x = bounds.xMin; x < bounds.xMax; x++)
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                    _roomOccupiedCells.Add(new Vector2Int(x, y));
        }

        // 방 사이의 연결 관계 순회
        var processedEdges = new HashSet<string>();
        foreach (var room in graph.allRooms)
        {
            foreach (var neighbor in room.neighbors)
            {
                string edgeKey = GetEdgeKey(room, neighbor);
                if (processedEdges.Contains(edgeKey)) continue;
                processedEdges.Add(edgeKey);

                var bestMatch = EvaluateBestConnection(room, neighbor);
                if (bestMatch != null) results.Add(bestMatch);
            }
        }
        return results;
    }

    private ConnectionResult EvaluateBestConnection(RoomNode a, RoomNode b)
    {
        var candidatesA = GetDoorCandidates(a);
        var candidatesB = GetDoorCandidates(b);

        ConnectionResult best = null;
        float minScore = float.MaxValue;

        foreach (var ca in candidatesA)
        {
            foreach (var cb in candidatesB)
            {
                // 두 문 후보(entrance) 사이의 경로 생성
                var path = GenerateGeometricPath(ca.entrance, cb.entrance);
                
                // 다른 방 통과 체크
                if (IsPathBlocked(path, a, b)) continue;

                // 점수 계산 (짧을수록, 굴절이 적을수록 좋음)
                float score = CalculateScore(path, ca, cb);
                if (score < minScore)
                {
                    minScore = score;
                    best = new ConnectionResult {
                        roomA = a, roomB = b, doorA = ca, doorB = cb, corridorPoints = path
                    };
                }
            }
        }
        return best;
    }

    // L자 경로 생성
    private List<Vector2Int> GenerateGeometricPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        
        // 시작점 추가
        Vector2Int current = start;
        path.Add(current);

        // X축 먼저 이동
        int xDir = (end.x >= start.x) ? 1 : -1;
        while (current.x != end.x)
        {
            current.x += xDir;
            path.Add(current);
        }

        // Y축 이동
        int yDir = (end.y >= start.y) ? 1 : -1;
        while (current.y != end.y)
        {
            current.y += yDir;
            path.Add(current);
        }

        return path;
    }

    private bool IsPathBlocked(List<Vector2Int> path, RoomNode ownerA, RoomNode ownerB)
    {
        RectInt boundsA = GetRoomBounds(ownerA);
        RectInt boundsB = GetRoomBounds(ownerB);

        foreach (var p in path)
        {
            // 점유된 셀이면서 연결 대상인 두 방의 내부가 아니라면 '차단'으로 간주
            if (_roomOccupiedCells.Contains(p))
            {
                if (!boundsA.Contains(p) && !boundsB.Contains(p)) return true;
            }
        }
        return false;
    }

    private float CalculateScore(List<Vector2Int> path, DoorCandidate da, DoorCandidate db)
    {
        float score = path.Count;

        // 문에서 나오자마자 꺾이는 것을 방지 (직진성 보장 가중치)
        if (path.Count > 1)
        {
            // 문 방향으로 첫 걸음이 나가지 않으면 페널티
            Vector2Int firstStepDir = path[1] - da.entrance;
            if (firstStepDir != DoorCandidate.GetOffset(da.dir)) score += 50f;

            // 마지막 걸음이 문 방향과 일치하지 않으면 페널티
            Vector2Int lastStepDir = db.entrance - path[path.Count - 2];
            if (lastStepDir != DoorCandidate.GetOffset(db.dir) * -1) score += 50f;
        }

        return score;
    }

    private List<DoorCandidate> GetDoorCandidates(RoomNode room)
    {
        var list = new List<DoorCandidate>();
        RectInt b = GetRoomBounds(room);

        if (room.roomData.useFixedDoor)
        {
            Vector2Int origin = new Vector2Int(Mathf.RoundToInt(room.worldPosition.x), Mathf.RoundToInt(room.worldPosition.y));
            list.Add(new DoorCandidate(origin + room.roomData.fixedDoorLocalPosition, (DoorDir)room.roomData.fixedDoorDirection, room));
            return list;
        }

        // 각 벽의 중앙 좌표 (정수 계산)
        list.Add(new DoorCandidate(new Vector2Int(b.xMin + b.width / 2, b.yMax - 1), DoorDir.Up, room));
        list.Add(new DoorCandidate(new Vector2Int(b.xMin + b.width / 2, b.yMin), DoorDir.Down, room));
        list.Add(new DoorCandidate(new Vector2Int(b.xMin, b.yMin + b.height / 2), DoorDir.Left, room));
        list.Add(new DoorCandidate(new Vector2Int(b.xMax - 1, b.yMin + b.height / 2), DoorDir.Right, room));

        return list;
    }

    private RectInt GetRoomBounds(RoomNode room) =>
        new RectInt(Mathf.RoundToInt(room.worldPosition.x), Mathf.RoundToInt(room.worldPosition.y), room.size.x, room.size.y);

    private string GetEdgeKey(RoomNode a, RoomNode b) => 
        string.Compare(a.nodeId, b.nodeId) < 0 ? $"{a.nodeId}_{b.nodeId}" : $"{b.nodeId}_{a.nodeId}";
}