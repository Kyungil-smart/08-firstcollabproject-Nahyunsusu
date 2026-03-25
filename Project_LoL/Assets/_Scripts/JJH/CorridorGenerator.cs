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

    [SerializeField] private float escapeDist = 2.0f; // 문 앞에서 얼마나 빠져나올지

    // 생성된 복도 렌더러 보관
    // 다음 Generate 호출 시 Clear()에서 정리
    private List<LineRenderer> _renderers = new List<LineRenderer>();
    private List<CorridorData> _corridors = new List<CorridorData>();

    public void Generate(MapGraph graph)
    {
        Clear();

        // A-B / B-A 중복 생성 방지
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

                // 문 로컬 좌표를 월드 좌표로 변환
                Vector2 start = room.worldPosition + door.localPosition;
                Vector2 end = door.connectedRoom.worldPosition + oppositeDoor.localPosition;

                Vector2[] points = BuildCorridorPath(
                    start,
                    end,
                    room,
                    door,
                    door.connectedRoom,
                    oppositeDoor,
                    graph);

                // 복도 데이터 저장
                CorridorData corridor = new CorridorData(room, door.connectedRoom, width, points);
                _corridors.Add(corridor);
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
        MapGraph graph)
    {
        List<Vector2> rawPath = new List<Vector2>();
        rawPath.Add(start);

        // 문 좌표를 바로 잇지 않고, 문 앞에서 한 번 빠져나온 지점을 기준으로 경로 생성
        Vector2 exitPos = new Vector2(
            Mathf.Round(start.x + GetDoorNormal(startDoor.direction).x * escapeDist),
            Mathf.Round(start.y + GetDoorNormal(startDoor.direction).y * escapeDist)
        );
        Vector2 entrancePos = new Vector2(
            Mathf.Round(end.x + GetDoorNormal(endDoor.direction).x * escapeDist),
            Mathf.Round(end.y + GetDoorNormal(endDoor.direction).y * escapeDist)
        );

        // exit와 entrance가 이미 같은 축에 있고 중간에 방이 없으면 직선 연결
        bool canConnectStraight = false;
        if (Mathf.Approximately(exitPos.x, entrancePos.x) || Mathf.Approximately(exitPos.y, entrancePos.y))
        {
            if (!IsPathBlocked(exitPos, entrancePos, graph, startRoom, endRoom, out _))
            {
                canConnectStraight = true;
            }
        }

        if (canConnectStraight)
        {
            rawPath.Add(exitPos);
            rawPath.Add(entrancePos);
        }
        else
        {
            // 시작 문 방향 기준으로 기본 mid 생성
            // 세로 문이면 x 고정, 가로 문이면 y 고정
            Vector2 mid;
            bool verticalStart = startDoor.direction == DoorDirection.Up || startDoor.direction == DoorDirection.Down;

            if (verticalStart)
                mid = new Vector2(exitPos.x, entrancePos.y);
            else
                mid = new Vector2(entrancePos.x, exitPos.y);

            // exit 이후 다시 뒤로 꺾이는 케이스 방지
            // 문 방향과 mid로 가는 방향이 반대면 다른 축 기준으로 다시 계산
            Vector2 exitDir = GetDoorNormal(startDoor.direction);
            Vector2 toMid = mid - exitPos;

            if (toMid.sqrMagnitude > 0.0001f)
            {
                Vector2 toMidDir = toMid.normalized;

                if (Vector2.Dot(exitDir, toMidDir) < 0f)
                {
                    if (verticalStart)
                        mid = new Vector2(entrancePos.x, exitPos.y);
                    else
                        mid = new Vector2(exitPos.x, entrancePos.y);
                }
            }

            // 첫 구간이 막히면 blocker 기준으로 mid를 밀어서 우회
            if (IsPathBlocked(exitPos, mid, graph, startRoom, endRoom, out RoomNode blocker1))
            {
                mid = CalculateBypass(exitPos, mid, blocker1, verticalStart);
            }
            // 두 번째 구간이 막히면 entrance 쪽 구간 기준으로 우회
            else if (IsPathBlocked(mid, entrancePos, graph, startRoom, endRoom, out RoomNode blocker2))
            {
                mid = CalculateBypass(mid, entrancePos, blocker2, !verticalStart);
            }

            // mid 최종 좌표를 타일 그리드에 맞게 정수화
            mid = new Vector2(
                Mathf.Round(mid.x),
                Mathf.Round(mid.y)
            );

            rawPath.Add(exitPos);
            rawPath.Add(mid);
            rawPath.Add(entrancePos);
        }

        rawPath.Add(end);

        // 같은 점이 연속으로 들어가면 불필요한 꺾임처럼 보일 수 있어서 제거
        List<Vector2> finalPath = new List<Vector2>();
        foreach (Vector2 p in rawPath)
        {
            // 모든 경로 좌표를 마지막에 한 번 더 정수화
            Vector2 rounded = new Vector2(Mathf.Round(p.x), Mathf.Round(p.y));

            if (finalPath.Count == 0 || Vector2.Distance(finalPath.Last(), rounded) > 0.01f)
            {
                finalPath.Add(rounded);
            }
        }

        return finalPath.ToArray();
    }

    private bool IsPathBlocked(
        Vector2 p1,
        Vector2 p2,
        MapGraph graph,
        RoomNode startRoom,
        RoomNode endRoom,
        out RoomNode blocker)
    {
        blocker = null;

        // 선분 그대로 검사하지 않고, 약간 두께가 있는 Rect로 검사
        // 너무 넓으면 과검사, 너무 좁으면 경계 케이스를 놓칠 수 있어서 적당히 유지
        float thickness = 0.5f;

        Rect pathRect = new Rect(
            Mathf.Min(p1.x, p2.x) - thickness,
            Mathf.Min(p1.y, p2.y) - thickness,
            Mathf.Abs(p1.x - p2.x) + (thickness * 2),
            Mathf.Abs(p1.y - p2.y) + (thickness * 2)
        );

        foreach (var room in graph.allRooms)
        {
            // 출발/도착 방은 연결 대상이므로 제외
            if (room == startRoom || room == endRoom)
                continue;

            if (pathRect.Overlaps(room.GetRect()))
            {
                blocker = room;
                return true;
            }
        }

        return false;
    }

    private Vector2 CalculateBypass(Vector2 p1, Vector2 mid, RoomNode blocker, bool isVerticalFirst)
    {
        Rect blockerRect = blocker.GetRect();
        Vector2 newMid = mid;

        if (isVerticalFirst)
        {
            // 세로 구간이 먼저라면 y축 기준으로 위/아래 중 가까운 쪽으로 우회
            float distToTop = Mathf.Abs(blockerRect.yMax - mid.y);
            float distToBottom = Mathf.Abs(blockerRect.yMin - mid.y);

            newMid.y = distToTop < distToBottom
                ? blockerRect.yMax + wallPadding
                : blockerRect.yMin - wallPadding;
        }
        else
        {
            // 가로 구간이 먼저라면 x축 기준으로 좌/우 중 가까운 쪽으로 우회
            float distToRight = Mathf.Abs(blockerRect.xMax - mid.x);
            float distToLeft = Mathf.Abs(blockerRect.xMin - mid.x);

            newMid.x = distToRight < distToLeft
                ? blockerRect.xMax + wallPadding
                : blockerRect.xMin - wallPadding;
        }

        // 우회 좌표도 타일 그리드에 맞게 정수화
        return new Vector2(
            Mathf.Round(newMid.x),
            Mathf.Round(newMid.y)
        );
    }

    private Vector2 GetDoorNormal(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.Up => Vector2.up,
            DoorDirection.Down => Vector2.down,
            DoorDirection.Left => Vector2.left,
            DoorDirection.Right => Vector2.right,
            _ => Vector2.zero
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

    public List<CorridorData> GetCorridors()
    {
        return _corridors;
    }
    
    private string GetConnectionKey(RoomNode a, RoomNode b)
    {
        return string.Compare(a.nodeId, b.nodeId) < 0
            ? $"{a.nodeId}_{b.nodeId}"
            : $"{b.nodeId}_{a.nodeId}";
    }

    private DoorData FindOppositeDoor(RoomNode room, RoomNode target)
    {
        foreach (var d in room.doors)
        {
            if (d.connectedRoom == target)
                return d;
        }

        return null;
    }
}