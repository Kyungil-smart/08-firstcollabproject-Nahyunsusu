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
            foreach (RoomNode neighbor in room.neighbors)
            {
                string key = GetConnectionKey(room, neighbor);

                if (processed.Contains(key))
                    continue;

                processed.Add(key);

                float width = Random.Range(minWidth, maxWidth);
                Vector2[] points = BuildCorridorPoints(room, neighbor);
                CorridorData corridor = new CorridorData(room, neighbor, width, points);

                _corridors.Add(corridor);
                DrawCorridor(corridor);
            }
        }
    }

    // 두 방 사이 복도 경로 계산
    // 같은 높이면 직선, 아니면 L자 꺾임
    private Vector2[] BuildCorridorPoints(RoomNode a, RoomNode b)
    {
        Vector2 start = a.worldPosition;
        Vector2 end   = b.worldPosition;

        // 방 중심이 아닌 경계 기준으로 복도 시작/끝 위치 계산
        float startY = start.y - a.size.y * 0.5f;
        float endY   = end.y   + b.size.y * 0.5f;

        Vector2 startPoint = new Vector2(start.x, startY);
        Vector2 endPoint   = new Vector2(end.x,   endY);
        
        float heightDiff = Mathf.Abs(start.y - end.y);

        // 높이 차이가 거의 없으면 직선
        if (heightDiff < 1f)
            return new Vector2[] { start, end };

        // L자: 수직 이동 후 수평 이동 (꺾임 지점을 시작점 Y 기준으로)
        Vector2 mid = new Vector2(endPoint.x, startPoint.y);
        return new Vector2[] { startPoint, mid, endPoint };
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