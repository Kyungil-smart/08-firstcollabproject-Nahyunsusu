using System.Collections.Generic;
using UnityEngine;

// 그래프의 연결 정보를 기반으로 복도를 생성
// LineRenderer로 시각화, 추후 타일맵/스프라이트로 교체 예정
public class CorridorGenerator : MonoBehaviour
{
    [Header("복도 설정")]
    public float minWidth = 2f;
    public float maxWidth = 6f;

    [Header("LineRenderer 설정")]
    public Material corridorMaterial;  // 없으면 Unity 기본 재질 사용

    private List<CorridorData> _corridors = new List<CorridorData>();
    private List<LineRenderer> _renderers = new List<LineRenderer>();

    // 현재 그래프 기준으로 복도 생성
    public void Generate(MapGraph graph)
    {
        Clear();

        // 중복 연결 방지용 (A→B, B→A 둘 다 처리되지 않도록)
        HashSet<string> processed = new HashSet<string>();

        foreach (RoomNode room in graph.allRooms)
        {
            foreach (DoorData door in room.doors)
            {
                string key = GetConnectionKey(room, door.connectedRoom);

                if (processed.Contains(key))
                    continue;

                processed.Add(key);
                
                // 이 문과 연결된 반대편 문 찾기
                DoorData oppositeDoor = FindOppositeDoor(door.connectedRoom, room);

                if (oppositeDoor == null)
                    continue;

                float width = Random.Range(minWidth, maxWidth);
                
                // 문 위치 기준으로 복도 경로 계산
                Vector2 startPos = room.worldPosition + door.localPosition;
                Vector2 endPos   = door.connectedRoom.worldPosition + oppositeDoor.localPosition;

                Vector2[] points = BuildCorridorPoints(startPos, endPos, door.direction);
                CorridorData corridor = new CorridorData(room, door.connectedRoom, width, points);

                _corridors.Add(corridor);
                DrawCorridor(corridor);
            }
        }
    }

    // 연결된 방에서 현재 방을 향하는 문 찾기
    private DoorData FindOppositeDoor(RoomNode room, RoomNode target)
    {
        foreach (DoorData door in room.doors)
        {
            if (door.connectedRoom == target)
                return door;
        }
        return null;
    }
    
    // 두 방 사이 복도 경로 계산
    // 수직 문이면 먼저 수직으로 나온 뒤 수평으로 이동, 수평 문이면 반대
    private Vector2[] BuildCorridorPoints(Vector2 start, Vector2 end, DoorDirection startDir)
    {
        float heightDiff = Mathf.Abs(start.y - end.y);
        float widthDiff  = Mathf.Abs(start.x - end.x);

        // 거의 직선이면 그냥 직선
        if (heightDiff < 1f && widthDiff < 1f)
            return new Vector2[] { start, end };

        // 문 방향 기준으로 꺾임 방향 결정
        Vector2 mid;
        if (startDir == DoorDirection.Up || startDir == DoorDirection.Down)
            mid = new Vector2(start.x, end.y);
        else
            mid = new Vector2(end.x, start.y);

        return new Vector2[] { start, mid, end };
    }

    private void DrawCorridor(CorridorData corridor)
    {
        GameObject obj = new GameObject($"Corridor_{corridor.roomA.nodeId}_to_{corridor.roomB.nodeId}");
        obj.transform.SetParent(transform);

        LineRenderer lr = obj.AddComponent<LineRenderer>();

        lr.positionCount = corridor.points.Length;
        lr.startWidth    = corridor.width;
        lr.endWidth      = corridor.width;
        lr.useWorldSpace = true;

        if (corridorMaterial != null)
            lr.material = corridorMaterial;

        for (int i = 0; i < corridor.points.Length; i++)
            lr.SetPosition(i, new Vector3(corridor.points[i].x, corridor.points[i].y, 0f));

        _renderers.Add(lr);
    }

    // 맵 재생성 전에 기존 복도 제거
    public void Clear()
    {
        foreach (LineRenderer lr in _renderers)
        {
            if (lr != null)
                Destroy(lr.gameObject);
        }

        _corridors.Clear();
        _renderers.Clear();
    }

    // 양방향 연결을 하나의 키로 통일
    private string GetConnectionKey(RoomNode a, RoomNode b)
    {
        string idA = a.nodeId;
        string idB = b.nodeId;

        return string.Compare(idA, idB) < 0 ? $"{idA}_{idB}" : $"{idB}_{idA}";
    }
}