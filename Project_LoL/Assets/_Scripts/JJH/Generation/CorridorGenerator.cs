using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    [Header("설정")]
    public float minWidth = 2f;
    public float maxWidth = 6f;
    public float wallPadding = 1.5f;
    public Material corridorMaterial;

    [SerializeField] private float _escapeDist = 4.0f;

    private List<LineRenderer> _renderers = new List<LineRenderer>();
    private List<CorridorData> _corridors = new List<CorridorData>();

    public void Generate(MapGraph graph)
    {
        Clear();

        HashSet<string> processed = new HashSet<string>();

        foreach (RoomNode room in graph.allRooms)
        {
            foreach (DoorData door in room.doors)
            {
                string key = GetConnectionKey(room, door.connectedRoom);
                if (processed.Contains(key))
                    continue;

                processed.Add(key);

                DoorData oppositeDoor = FindOppositeDoor(door.connectedRoom, room);
                if (oppositeDoor == null)
                    continue;

                float width = Mathf.Round(Random.Range(minWidth, maxWidth));
                int intWidth = Mathf.Max(1, Mathf.RoundToInt(width));

                door.openingWidth = intWidth;
                oppositeDoor.openingWidth = intWidth;

                Vector2 start = room.worldPosition + door.localPosition;
                Vector2 end   = door.connectedRoom.worldPosition + oppositeDoor.localPosition;

                Vector2[] points = BuildCorridorPath(
                    start, end,
                    room, door,
                    door.connectedRoom, oppositeDoor,
                    graph, width);

                if (points == null)
                {
                    Debug.LogWarning($"[CorridorGenerator] {room.nodeId} → {door.connectedRoom.nodeId} 유효한 경로 없음");
                    continue;
                }

                _corridors.Add(new CorridorData(room, door.connectedRoom, width, points));
            }
        }
    }

    private Vector2[] BuildCorridorPath(
        Vector2 start,
        Vector2 end,
        RoomNode startRoom,
        DoorData startDoor,
        RoomNode endRoom,
        DoorData endDoor,
        MapGraph graph,
        float corridorWidth)
    {
        float halfWidth = corridorWidth * 0.5f;

        // 문 앞에서 escapeDist만큼 빠져나온 지점
        Vector2 exitPos = SnapToGrid(start + GetDoorNormal(startDoor.direction) * _escapeDist);
        Vector2 entrancePos = SnapToGrid(end + GetDoorNormal(endDoor.direction) * _escapeDist);

        // 직선 연결 가능 여부 먼저 확인
        if (CanConnectStraight(exitPos, entrancePos, graph, startRoom, endRoom, halfWidth))
        {
            return BuildPath(start, exitPos, entrancePos, end);
        }

        // 후보 A: exit.x 기준 → entrance.y 기준 꺾임
        Vector2 midA = new Vector2(exitPos.x, entrancePos.y);

        // 후보 B: entrance.x 기준 → exit.y 기준 꺾임
        Vector2 midB = new Vector2(entrancePos.x, exitPos.y);

        bool validA = IsValidPath(exitPos, midA, entrancePos, graph, startRoom, endRoom, halfWidth);
        bool validB = IsValidPath(exitPos, midB, entrancePos, graph, startRoom, endRoom, halfWidth);

        if (validA && validB)
        {
            // 둘 다 유효하면 더 짧은 경로 선택
            float lenA = PathLength(exitPos, midA, entrancePos);
            float lenB = PathLength(exitPos, midB, entrancePos);
            Vector2 mid = lenA <= lenB ? midA : midB;
            return BuildPath(start, exitPos, SnapToGrid(mid), entrancePos, end);
        }

        if (validA)
            return BuildPath(start, exitPos, SnapToGrid(midA), entrancePos, end);

        if (validB)
            return BuildPath(start, exitPos, SnapToGrid(midB), entrancePos, end);

        // 둘 다 막힘
        return null;
    }

    // 직선 연결 가능 여부 확인
    private bool CanConnectStraight(
        Vector2 exitPos, Vector2 entrancePos,
        MapGraph graph, RoomNode startRoom, RoomNode endRoom,
        float halfWidth)
    {
        if (!Mathf.Approximately(exitPos.x, entrancePos.x) &&
            !Mathf.Approximately(exitPos.y, entrancePos.y))
            return false;

        return !IsPathBlocked(exitPos, entrancePos, graph, startRoom, endRoom, halfWidth);
    }

    // L자 경로 전체 유효 여부 확인
    private bool IsValidPath(
        Vector2 exitPos, Vector2 mid, Vector2 entrancePos,
        MapGraph graph, RoomNode startRoom, RoomNode endRoom,
        float halfWidth)
    {
        return !IsPathBlocked(exitPos, mid, graph, startRoom, endRoom, halfWidth) &&
               !IsPathBlocked(mid, entrancePos, graph, startRoom, endRoom, halfWidth);
    }

    // 경로 총 길이 계산
    private float PathLength(Vector2 exitPos, Vector2 mid, Vector2 entrancePos)
    {
        return Vector2.Distance(exitPos, mid) + Vector2.Distance(mid, entrancePos);
    }

    // 포인트들로 최종 경로 배열 생성
    // 중복 좌표 제거
    private Vector2[] BuildPath(params Vector2[] points)
    {
        List<Vector2> result = new List<Vector2>();

        foreach (Vector2 p in points)
        {
            Vector2 snapped = SnapToGrid(p);
            if (result.Count == 0 || Vector2.Distance(result.Last(), snapped) > 0.01f)
                result.Add(snapped);
        }

        return result.ToArray();
    }

    private bool IsPathBlocked(
        Vector2 p1, Vector2 p2,
        MapGraph graph,
        RoomNode startRoom, RoomNode endRoom,
        float halfWidth)
    {
        float thickness = Mathf.Max(0.5f, halfWidth);

        Rect pathRect = new Rect(
            Mathf.Min(p1.x, p2.x) - thickness,
            Mathf.Min(p1.y, p2.y) - thickness,
            Mathf.Abs(p1.x - p2.x) + thickness * 2,
            Mathf.Abs(p1.y - p2.y) + thickness * 2
        );

        foreach (RoomNode room in graph.allRooms)
        {
            // 출발/도착 방은 연결 대상이므로 제외
            if (room == startRoom || room == endRoom)
                continue;

            Rect roomRect = new Rect(
                room.GetRect().x - wallPadding,
                room.GetRect().y - wallPadding,
                room.GetRect().width + wallPadding * 2,
                room.GetRect().height + wallPadding * 2
            );

            if (pathRect.Overlaps(roomRect))
                return true;
        }

        return false;
    }

    private Vector2 SnapToGrid(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    private Vector2 GetDoorNormal(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.Up    => Vector2.up,
            DoorDirection.Down  => Vector2.down,
            DoorDirection.Left  => Vector2.left,
            DoorDirection.Right => Vector2.right,
            _                   => Vector2.zero
        };
    }

    public void Clear()
    {
        foreach (var lr in _renderers)
        {
            if (lr)
                Destroy(lr.gameObject);
        }

        _renderers.Clear();
        _corridors.Clear();
    }

    public List<CorridorData> GetCorridors() => _corridors;

    private string GetConnectionKey(RoomNode a, RoomNode b)
    {
        return string.Compare(a.nodeId, b.nodeId) < 0
            ? $"{a.nodeId}_{b.nodeId}"
            : $"{b.nodeId}_{a.nodeId}";
    }

    private DoorData FindOppositeDoor(RoomNode room, RoomNode target)
    {
        foreach (DoorData d in room.doors)
        {
            if (d.connectedRoom == target)
                return d;
        }

        return null;
    }
}