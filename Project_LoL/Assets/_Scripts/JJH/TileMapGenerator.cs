using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour
{
    [Header("타일 프리팹")]
    public GameObject floorTilePrefab;
    public GameObject wallTilePrefab;

    [Header("벽 설정")]
    public int wallThickness = 2;

    private List<GameObject> _tiles = new List<GameObject>();

    public void Generate(MapGraph graph, List<CorridorData> corridors)
    {
        Clear();

        // 좌표 먼저 수집한 뒤 한 번에 생성
        HashSet<Vector2Int> floorSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> wallSet = new HashSet<Vector2Int>();

        foreach (RoomNode room in graph.allRooms)
        {
            if (room.size.x == 0 || room.size.y == 0)
                continue;

            CollectRoomFloor(room, floorSet);
            CollectRoomWall(room, wallSet);
        }

        foreach (CorridorData corridor in corridors)
        {
            CollectCorridorFloor(corridor, floorSet);
            CollectCorridorWall(corridor, wallSet);
        }

        // 바닥이 있는 곳은 벽에서 제외
        wallSet.ExceptWith(floorSet);

        foreach (Vector2Int pos in floorSet)
        {
            PlaceTile(floorTilePrefab, new Vector2(pos.x + 0.5f, pos.y + 0.5f));
        }

        foreach (Vector2Int pos in wallSet)
        {
            PlaceTile(wallTilePrefab, new Vector2(pos.x + 0.5f, pos.y + 0.5f));
        }
    }

    // 방 내부 바닥 수집
    private void CollectRoomFloor(RoomNode room, HashSet<Vector2Int> set)
    {
        int startX = Mathf.RoundToInt(room.worldPosition.x - room.size.x * 0.5f);
        int startY = Mathf.RoundToInt(room.worldPosition.y - room.size.y * 0.5f);

        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
            {
                set.Add(new Vector2Int(startX + x, startY + y));
            }
        }
    }

    // 방 외곽 벽 수집
    private void CollectRoomWall(RoomNode room, HashSet<Vector2Int> set)
{
    int startX = Mathf.RoundToInt(room.worldPosition.x - room.size.x * 0.5f);
    int startY = Mathf.RoundToInt(room.worldPosition.y - room.size.y * 0.5f);
    int endX = startX + room.size.x;
    int endY = startY + room.size.y;

    // 문이 있는 구간은 방 벽에서 제외
    HashSet<Vector2Int> doorPositions = new HashSet<Vector2Int>();
    int halfDoorWidth = 2; // 문 반폭

    foreach (DoorData door in room.doors)
    {
        Vector2 worldDoorPos = room.worldPosition + door.localPosition;
        int dx = Mathf.RoundToInt(worldDoorPos.x);
        int dy = Mathf.RoundToInt(worldDoorPos.y);

        switch (door.direction)
        {
            case DoorDirection.Up:
            case DoorDirection.Down:
                for (int i = -halfDoorWidth; i <= halfDoorWidth; i++)
                {
                    doorPositions.Add(new Vector2Int(dx + i, dy));
                }
                break;

            case DoorDirection.Left:
            case DoorDirection.Right:
                for (int i = -halfDoorWidth; i <= halfDoorWidth; i++)
                {
                    doorPositions.Add(new Vector2Int(dx, dy + i));
                }
                break;
        }
    }

    for (int w = 1; w <= wallThickness; w++)
    {
        for (int x = startX - w; x < endX + w; x++)
        {
            Vector2Int top = new Vector2Int(x, endY + w - 1);
            Vector2Int bottom = new Vector2Int(x, startY - w);

            if (!doorPositions.Contains(top))
                set.Add(top);

            if (!doorPositions.Contains(bottom))
                set.Add(bottom);
        }

        for (int y = startY - w + 1; y < endY + w - 1; y++)
        {
            Vector2Int left = new Vector2Int(startX - w, y);
            Vector2Int right = new Vector2Int(endX + w - 1, y);

            if (!doorPositions.Contains(left))
                set.Add(left);

            if (!doorPositions.Contains(right))
                set.Add(right);
        }
    }
}

    // 복도 바닥 수집
    private void CollectCorridorFloor(CorridorData corridor, HashSet<Vector2Int> set)
    {
        int halfWidth = Mathf.RoundToInt(corridor.width * 0.5f);

        for (int i = 0; i < corridor.points.Length - 1; i++)
        {
            CollectSegment(corridor.points[i], corridor.points[i + 1], halfWidth, set);
        }
    }

    // 복도 외곽 벽 수집
    private void CollectCorridorWall(CorridorData corridor, HashSet<Vector2Int> set)
    {
        int halfWidth = Mathf.RoundToInt(corridor.width * 0.5f);
        int wallOuterEnd = halfWidth + wallThickness;

        for (int i = 0; i < corridor.points.Length - 1; i++)
        {
            Vector2 from = corridor.points[i];
            Vector2 to = corridor.points[i + 1];

            for (int w = halfWidth + 1; w <= wallOuterEnd; w++)
            {
                CollectSegmentWall(from, to, w, set);
            }

            // 꺾이는 지점 벽 보강
            if (i < corridor.points.Length - 2)
            {
                CollectCornerTiles(corridor.points[i + 1], wallOuterEnd, set);
            }
        }
    }

    // 코너 주변 벽 보강
    private void CollectCornerTiles(Vector2 corner, int wallOuter, HashSet<Vector2Int> set)
    {
        int cx = Mathf.RoundToInt(corner.x);
        int cy = Mathf.RoundToInt(corner.y);

        for (int dx = -wallOuter; dx <= wallOuter; dx++)
        {
            for (int dy = -wallOuter; dy <= wallOuter; dy++)
            {
                set.Add(new Vector2Int(cx + dx, cy + dy));
            }
        }
    }

    // 직선 구간 바닥 수집
    // 현재 복도 경로는 수평/수직 전제
    private void CollectSegment(Vector2 from, Vector2 to, int halfWidth, HashSet<Vector2Int> set)
    {
        bool isHorizontal = Mathf.Abs(to.y - from.y) < 0.1f;

        int startX = Mathf.RoundToInt(Mathf.Min(from.x, to.x));
        int startY = Mathf.RoundToInt(Mathf.Min(from.y, to.y));
        int endX = Mathf.RoundToInt(Mathf.Max(from.x, to.x));
        int endY = Mathf.RoundToInt(Mathf.Max(from.y, to.y));

        if (isHorizontal)
        {
            for (int x = startX; x < endX; x++)
            {
                for (int w = -halfWidth; w < halfWidth; w++)
                {
                    set.Add(new Vector2Int(x, startY + w));
                }
            }
        }
        else
        {
            for (int y = startY; y < endY; y++)
            {
                for (int w = -halfWidth; w < halfWidth; w++)
                {
                    set.Add(new Vector2Int(startX + w, y));
                }
            }
        }
    }

    // 직선 구간 외곽 벽 수집
    private void CollectSegmentWall(Vector2 from, Vector2 to, int offset, HashSet<Vector2Int> set)
    {
        bool isHorizontal = Mathf.Abs(to.y - from.y) < 0.1f;

        int startX = Mathf.RoundToInt(Mathf.Min(from.x, to.x));
        int startY = Mathf.RoundToInt(Mathf.Min(from.y, to.y));
        int endX = Mathf.RoundToInt(Mathf.Max(from.x, to.x));
        int endY = Mathf.RoundToInt(Mathf.Max(from.y, to.y));

        if (isHorizontal)
        {
            for (int x = startX; x < endX; x++)
            {
                set.Add(new Vector2Int(x, startY + offset));
                set.Add(new Vector2Int(x, startY - offset));
            }
        }
        else
        {
            for (int y = startY; y < endY; y++)
            {
                set.Add(new Vector2Int(startX + offset, y));
                set.Add(new Vector2Int(startX - offset, y));
            }
        }
    }

    private void PlaceTile(GameObject prefab, Vector2 position)
    {
        if (prefab == null)
            return;

        GameObject tile = Instantiate(prefab, new Vector3(position.x, position.y, 0f), Quaternion.identity);
        tile.transform.SetParent(transform);
        _tiles.Add(tile);
    }

    public void Clear()
    {
        foreach (GameObject tile in _tiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }

        _tiles.Clear();
    }
}