using System.Collections.Generic;
using UnityEngine;

public class CorridorGenerator : MonoBehaviour
{
    [Header("설정")]
    public float minWidth = 2f;
    public float maxWidth = 6f;
    public float wallPadding = 1.5f;
    public Material corridorMaterial;
    
    [SerializeField] float escapeDist = 2.0f;

    // 생성한 복도 렌더러들 보관
    // 다음 재생성 때 Clear()에서 한 번에 지우기 위해 저장해둠
    private List<LineRenderer> _renderers = new List<LineRenderer>();

    public void Generate(MapGraph graph)
    {
        // 이전에 만든 복도 먼저 정리
        Clear();

        // A-B, B-A 중복 생성 방지용
        HashSet<string> processed = new HashSet<string>();

        foreach (RoomNode room in graph.allRooms)
        {
            foreach (DoorData door in room.doors)
            {
                // 방 연결 쌍을 하나의 키로 만들어 중복 체크
                string key = GetConnectionKey(room, door.connectedRoom);
                if (processed.Contains(key))
                    continue;

                processed.Add(key);

                // 반대편 방에서 현재 room으로 연결되는 문 찾기
                // 문이 없으면 정상 연결이 안 되는 상태이므로 스킵
                DoorData oppositeDoor = FindOppositeDoor(door.connectedRoom, room);
                if (oppositeDoor == null)
                    continue;

                // 복도 폭은 범위 내 랜덤
                float width = Random.Range(minWidth, maxWidth);
                
                // 각 문의 로컬 좌표를 방 월드 좌표에 더해서 실제 월드 위치 계산
                Vector2 start = room.worldPosition + door.localPosition;
                Vector2 end = door.connectedRoom.worldPosition + oppositeDoor.localPosition;

                // 문 → 문 사이에 들어갈 복도 경로 계산
                Vector2[] points = BuildCorridorPath(start, end, room, door, door.connectedRoom, oppositeDoor, graph);
                
                // 계산된 포인트를 기준으로 실제 라인 렌더러 생성
                DrawCorridor(points, width, room.nodeId, door.connectedRoom.nodeId);
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
        List<Vector2> path = new List<Vector2>();
        
        // 문 바로 앞에서 바로 꺾으면 방 벽과 너무 붙어 보일 수 있어서
        // 문 방향으로 조금 더 전진한 지점을 기준으로 복도 경로를 잡음
        // float escapeDist = 2.0f;

        // 시작 방 문에서 바깥쪽으로 한 번 빠져나온 지점
        Vector2 exitPos = start + GetDoorNormal(startDoor.direction) * escapeDist;
        
        // 도착 방 문에 들어가기 전에 바깥쪽에서 한 번 꺾일 지점
        Vector2 entrancePos = end + GetDoorNormal(endDoor.direction) * escapeDist;

        // 실제 복도는 문 위치부터 시작하므로 시작 문 좌표 추가
        path.Add(start);
        
        // 문 앞에서 한 칸 빠진 지점 추가
        path.Add(exitPos);

        Vector2 mid;
        
        // 시작 문 방향이 세로(Up/Down)면
        // x는 시작 쪽, y는 도착 쪽 값을 써서 ㄱ자 꺾임 생성
        if (startDoor.direction == DoorDirection.Up || startDoor.direction == DoorDirection.Down)
            mid = new Vector2(exitPos.x, entrancePos.y);
        else
            // 시작 문 방향이 가로(Left/Right)면 반대로 계산
            mid = new Vector2(entrancePos.x, exitPos.y);

        // exitPos -> mid 구간이 다른 방과 겹치면
        // mid를 반대 축 방향으로 밀어서 우회 시도
        if (IsPathBlocked(exitPos, mid, graph, startRoom, endRoom, out RoomNode blocker1))
        {
            mid = CalculateBypass(exitPos, mid, blocker1, true);
        }
        // 첫 구간은 괜찮고 mid -> entrancePos 구간이 막히면
        // 이쪽도 같은 방식으로 우회 시도
        else if (IsPathBlocked(mid, entrancePos, graph, startRoom, endRoom, out RoomNode blocker2))
        {
            mid = CalculateBypass(mid, entrancePos, blocker2, false);
        }

        // 꺾이는 중간 지점
        path.Add(mid);

        // 도착 문 앞 지점
        path.Add(entrancePos);

        // 최종 도착 문 위치
        path.Add(end);
        
        string pathLog = "";
        for (int i = 0; i < path.Count; i++)
        {
            pathLog += $"[{i}] {path[i]} ";
        }

        Debug.Log($"start={start}, exit={exitPos}, mid={mid}, entrance={entrancePos}, end={end}");
        Debug.Log($"path: {pathLog}");
        
        return path.ToArray();
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

        // 선분 자체로 검사하지 않고,
        // 약간 두께가 있는 직사각형으로 만들어 방과 겹치는지 확인
        // 너무 얇으면 경계에 걸치는 경우를 놓칠 수 있어서 여유를 둠
        float thickness = 1.0f;
        Rect pathRect = new Rect(
            Mathf.Min(p1.x, p2.x) - thickness,
            Mathf.Min(p1.y, p2.y) - thickness,
            Mathf.Abs(p1.x - p2.x) + (thickness * 2),
            Mathf.Abs(p1.y - p2.y) + (thickness * 2)
        );

        foreach (var room in graph.allRooms)
        {
            // 출발 방, 도착 방은 원래 연결 대상이므로 충돌 검사에서 제외
            if (room == startRoom || room == endRoom)
                continue;

            // 경로 직사각형이 다른 방 Rect와 겹치면 막힌 것으로 판단
            if (pathRect.Overlaps(room.GetRect()))
            {
                blocker = room;
                return true;
            }
        }

        return false;
    }

    private Vector2 CalculateBypass(Vector2 p1, Vector2 mid, RoomNode blocker, bool isHorizontalFirst)
    {
        Rect blockerRect = blocker.GetRect();
        Vector2 newMid = mid;

        if (isHorizontalFirst)
        {
            // 첫 구간이 가로 중심 이동이었다면
            // y축 방향으로 위/아래 중 더 가까운 쪽으로 피해감
            float distToTop = Mathf.Abs(blockerRect.yMax - mid.y);
            float distToBottom = Mathf.Abs(blockerRect.yMin - mid.y);

            newMid.y = distToTop < distToBottom
                ? blockerRect.yMax + wallPadding
                : blockerRect.yMin - wallPadding;
        }
        else
        {
            // 첫 구간이 세로 중심 이동이었다면
            // x축 방향으로 좌/우 중 더 가까운 쪽으로 피해감
            float distToRight = Mathf.Abs(blockerRect.xMax - mid.x);
            float distToLeft = Mathf.Abs(blockerRect.xMin - mid.x);

            newMid.x = distToRight < distToLeft
                ? blockerRect.xMax + wallPadding
                : blockerRect.xMin - wallPadding;
        }

        return newMid;
    }

    private Vector2 GetDoorNormal(DoorDirection dir)
    {
        // 문의 방향을 바깥쪽 단위 벡터로 변환
        // escapeDist를 곱해서 문 앞 지점을 계산할 때 사용
        return dir switch
        {
            DoorDirection.Up => Vector2.up,
            DoorDirection.Down => Vector2.down,
            DoorDirection.Left => Vector2.left,
            DoorDirection.Right => Vector2.right,
            _ => Vector2.zero
        };
    }

    private void DrawCorridor(Vector2[] points, float width, string idA, string idB)
    {
        // 연결 관계가 보이도록 이름 부여
        GameObject obj = new GameObject($"Corridor_{idA}_to_{idB}");
        obj.transform.SetParent(transform);

        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.positionCount = points.Length;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.useWorldSpace = true;

        if (corridorMaterial)
            lr.material = corridorMaterial;

        // 계산된 2D 포인트를 LineRenderer용 3D 좌표로 넣음
        // 현재는 XY 평면 기준이라 z는 0 고정
        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, new Vector3(points[i].x, points[i].y, 0f));
        }

        _renderers.Add(lr);
    }

    public void Clear()
    {
        // 기존 복도 오브젝트 제거
        foreach (var lr in _renderers)
        {
            if (lr)
                Destroy(lr.gameObject);
        }

        _renderers.Clear();
    }

    private string GetConnectionKey(RoomNode a, RoomNode b)
    {
        // A-B, B-A를 같은 연결로 취급하기 위해
        // nodeId를 정렬해서 항상 같은 문자열 키 생성
        return string.Compare(a.nodeId, b.nodeId) < 0
            ? $"{a.nodeId}_{b.nodeId}"
            : $"{b.nodeId}_{a.nodeId}";
    }

    private DoorData FindOppositeDoor(RoomNode room, RoomNode target)
    {
        // room 쪽 문들 중에서 target 방으로 연결된 문 찾기
        foreach (var d in room.doors)
        {
            if (d.connectedRoom == target)
                return d;
        }

        return null;
    }
}