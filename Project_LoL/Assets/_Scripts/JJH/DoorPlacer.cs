using System.Collections.Generic;
using UnityEngine;

// 방 연결 방향에 따라 문 위치 계산
// 방향 판정 후 벽면에 문 배치, 같은 벽면에 여러 문이면 균등 배치
public class DoorPlacer : MonoBehaviour
{
    public void PlaceDoors(MapGraph graph)
    {
        // 모든 방의 문 목록 초기화
        foreach (RoomNode room in graph.allRooms)
            room.doors.Clear();

        // 중복 처리 방지
        HashSet<string> processed = new HashSet<string>();

        foreach (RoomNode room in graph.allRooms)
        {
            foreach (RoomNode neighbor in room.neighbors)
            {
                string key = GetConnectionKey(room, neighbor);

                if (processed.Contains(key))
                    continue;

                processed.Add(key);

                // 두 방 사이 방향 판정
                DoorDirection dirFromA = GetDirection(room, neighbor);
                DoorDirection dirFromB = GetOpposite(dirFromA);

                // 각 방에 문 데이터 추가
                room.doors.Add(new DoorData(neighbor, dirFromA, Vector2.zero));
                neighbor.doors.Add(new DoorData(room, dirFromB, Vector2.zero));
            }
        }

        // 방향별 문 위치 계산
        foreach (RoomNode room in graph.allRooms)
            CalculateDoorPositions(room);
    }

    // 두 방 중심 좌표 차이 기준으로 방향 판정
    private DoorDirection GetDirection(RoomNode from, RoomNode to)
    {
        Vector2 diff = to.worldPosition - from.worldPosition;
        
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            return diff.x > 0 ? DoorDirection.Right : DoorDirection.Left;
        }

        return diff.y > 0 ? DoorDirection.Up : DoorDirection.Down;
    }

    private DoorDirection GetOpposite(DoorDirection dir)
    {
        switch (dir)
        {
            case DoorDirection.Up:    return DoorDirection.Down;
            case DoorDirection.Down:  return DoorDirection.Up;
            case DoorDirection.Left:  return DoorDirection.Right;
            case DoorDirection.Right: return DoorDirection.Left;
            default:                  return DoorDirection.Up;
        }
    }

    // 같은 방향 벽면에 문이 여러 개면 균등 배치
    private void CalculateDoorPositions(RoomNode room)
    {
        // 방향별로 문 묶기
        Dictionary<DoorDirection, List<DoorData>> grouped
            = new Dictionary<DoorDirection, List<DoorData>>();

        grouped[DoorDirection.Up] = new List<DoorData>();
        grouped[DoorDirection.Down] = new List<DoorData>();
        grouped[DoorDirection.Left] = new List<DoorData>();
        grouped[DoorDirection.Right] = new List<DoorData>();

        foreach (DoorData door in room.doors)
            grouped[door.direction].Add(door);

        float halfW = room.size.x * 0.5f;
        float halfH = room.size.y * 0.5f;

        foreach (KeyValuePair<DoorDirection, List<DoorData>> pair in grouped)
        {
            List<DoorData> doors = pair.Value;

            if (doors.Count == 0)
                continue;

            DoorDirection dir = pair.Key;
            int count = doors.Count;

            for (int i = 0; i < count; i++)
            {
                // 벽면 길이 기준으로 균등 분할
                float t = (i + 1f) / (count + 1f);
                Vector2 localPos = Vector2.zero;

                switch (dir)
                {
                    case DoorDirection.Up:
                        localPos = new Vector2(Mathf.Lerp(-halfW, halfW, t), halfH);
                        break;
                    case DoorDirection.Down:
                        localPos = new Vector2(Mathf.Lerp(-halfW, halfW, t), -halfH);
                        break;
                    case DoorDirection.Left:
                        localPos = new Vector2(-halfW, Mathf.Lerp(-halfH, halfH, t));
                        break;
                    case DoorDirection.Right:
                        localPos = new Vector2(halfW, Mathf.Lerp(-halfH, halfH, t));
                        break;
                }

                doors[i].localPosition = localPos;
            }
        }
    }

    private string GetConnectionKey(RoomNode a, RoomNode b)
    {
        return string.Compare(a.nodeId, b.nodeId) < 0
            ? $"{a.nodeId}_{b.nodeId}"
            : $"{b.nodeId}_{a.nodeId}";
    }
}