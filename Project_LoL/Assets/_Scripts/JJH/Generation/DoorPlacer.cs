using System.Collections.Generic;
using UnityEngine;

public class DoorPlacer : MonoBehaviour
{
    [SerializeField] private float _doorMargin = 1.5f; // 벽 끝에 너무 붙지 않도록 여유
    [SerializeField] private int _maxCorridorWidth = 6; // 최대 복도 폭 (문 위치 보정용)

    public void PlaceDoors(MapGraph graph)
    {
        // 기존 문 정보 초기화
        foreach (RoomNode room in graph.allRooms)
            room.doors.Clear();

        HashSet<string> processed = new HashSet<string>(); // 양방향 중복 방지

        foreach (RoomNode room in graph.allRooms)
        {
            foreach (RoomNode neighbor in room.neighbors)
            {
                string key = GetConnectionKey(room, neighbor);
                if (processed.Contains(key))
                    continue;

                processed.Add(key);

                // 두 방 사이의 상대 방향 계산
                DoorDirection dirFromA = GetDirection(room, neighbor);
                DoorDirection dirFromB = GetOpposite(dirFromA);

                // 상대 방 중심 기준으로 문 위치 계산 (균등 배치 대신 축 정렬)
                Vector2 posA = CalculateClampedLocalPos(room, neighbor, dirFromA);
                Vector2 posB = CalculateClampedLocalPos(neighbor, room, dirFromB);

                room.doors.Add(new DoorData(neighbor, dirFromA, posA));
                neighbor.doors.Add(new DoorData(room, dirFromB, posB));
            }
        }
    }

    private Vector2 CalculateClampedLocalPos(RoomNode owner, RoomNode target, DoorDirection dir)
    {
        float halfW = owner.size.x * 0.5f;
        float halfH = owner.size.y * 0.5f;

        // 복도 폭을 고려해 코너에 붙지 않도록 추가 여유 확보
        float corridorHalf = Mathf.Floor(_maxCorridorWidth * 0.5f);
        float safeMargin = Mathf.Max(_doorMargin, corridorHalf);

        // 작은 방에서도 범위가 뒤집히지 않도록 보정
        float xMin = Mathf.Min(-halfW + safeMargin, halfW - safeMargin);
        float xMax = Mathf.Max(-halfW + safeMargin, halfW - safeMargin);
        float yMin = Mathf.Min(-halfH + safeMargin, halfH - safeMargin);
        float yMax = Mathf.Max(-halfH + safeMargin, halfH - safeMargin);

        // target 위치를 owner 기준 로컬 좌표로 변환
        Vector2 relativePos = target.worldPosition - owner.worldPosition;
        Vector2 localPos = Vector2.zero;

        switch (dir)
        {
            case DoorDirection.Up:
            case DoorDirection.Down:
                // 상하 연결 → X축 기준으로 정렬
                float clampX = Mathf.Clamp(relativePos.x, xMin, xMax);
                localPos = new Vector2(clampX, dir == DoorDirection.Up ? halfH : -halfH);
                break;

            case DoorDirection.Left:
            case DoorDirection.Right:
                // 좌우 연결 → Y축 기준으로 정렬
                float clampY = Mathf.Clamp(relativePos.y, yMin, yMax);
                localPos = new Vector2(dir == DoorDirection.Right ? halfW : -halfW, clampY);
                break;
        }

        localPos = new Vector2(Mathf.Round(localPos.x), Mathf.Round(localPos.y));
        return localPos;
    }

    private DoorDirection GetDirection(RoomNode from, RoomNode to)
    {
        Rect fromRect = from.GetRect();
        Rect toRect = to.GetRect();

        bool overlapX = fromRect.xMin < toRect.xMax && fromRect.xMax > toRect.xMin;
        bool overlapY = fromRect.yMin < toRect.yMax && fromRect.yMax > toRect.yMin;

        Vector2 diff = to.worldPosition - from.worldPosition;

        // x축 범위가 겹치면 위/아래로 연결하는 게 자연스러움
        if (overlapX && !overlapY)
            return diff.y > 0 ? DoorDirection.Up : DoorDirection.Down;

        // y축 범위가 겹치면 좌/우로 연결하는 게 자연스러움
        if (overlapY && !overlapX)
            return diff.x > 0 ? DoorDirection.Right : DoorDirection.Left;

        // 둘 다 겹치거나 둘 다 안 겹치면 기존 방식 fallback
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return diff.x > 0 ? DoorDirection.Right : DoorDirection.Left;

        return diff.y > 0 ? DoorDirection.Up : DoorDirection.Down;
    }

    private DoorDirection GetOpposite(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.Up => DoorDirection.Down,
            DoorDirection.Down => DoorDirection.Up,
            DoorDirection.Left => DoorDirection.Right,
            DoorDirection.Right => DoorDirection.Left,
            _ => DoorDirection.Up
        };
    }

    private string GetConnectionKey(RoomNode a, RoomNode b)
    {
        // 같은 연결을 두 번 처리하지 않도록 정렬 키 생성
        return string.Compare(a.nodeId, b.nodeId) < 0
            ? $"{a.nodeId}_{b.nodeId}"
            : $"{b.nodeId}_{a.nodeId}";
    }
}