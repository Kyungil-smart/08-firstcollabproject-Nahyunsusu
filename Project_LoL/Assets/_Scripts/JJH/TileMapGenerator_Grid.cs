using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator_Grid : MonoBehaviour
{
    public enum CellType { Empty, Floor, Door }

    [Header("타일 프리팹")]
    public GameObject floorTilePrefab;
    public GameObject wallTilePrefab;

    [Header("벽 설정")]
    public int wallThickness = 2;

    private Dictionary<Vector2Int, CellType> _grid = new Dictionary<Vector2Int, CellType>();
    private List<GameObject> _tiles = new List<GameObject>();

    public void Generate(MapGraph graph, List<CorridorData> corridors)
    {
        Clear();

        // 방/복도 바닥 먼저 마킹
        foreach (RoomNode room in graph.allRooms)
        {
            if (room.size.x == 0 || room.size.y == 0)
                continue;

            MarkRoomFloor(room);
        }

        foreach (CorridorData corridor in corridors)
            MarkCorridorFloor(corridor);

        // 문 위치 마킹
        foreach (RoomNode room in graph.allRooms)
            MarkDoors(room);

        // 바닥/문 주변 빈 칸을 벽으로 처리
        HashSet<Vector2Int> wallSet = new HashSet<Vector2Int>();

        foreach (KeyValuePair<Vector2Int, CellType> cell in _grid)
        {
            if (cell.Value != CellType.Floor && cell.Value != CellType.Door)
                continue;

            for (int w = 1; w <= wallThickness; w++)
            {
                foreach (Vector2Int neighbor in GetNeighbors(cell.Key, w))
                {
                    if (!_grid.ContainsKey(neighbor))
                        wallSet.Add(neighbor);
                }
            }
        }

        foreach (KeyValuePair<Vector2Int, CellType> cell in _grid)
            PlaceTile(floorTilePrefab, cell.Key);

        foreach (Vector2Int pos in wallSet)
            PlaceTile(wallTilePrefab, pos);
    }

    private void MarkRoomFloor(RoomNode room)
    {
        int startX = Mathf.RoundToInt(room.worldPosition.x - room.size.x * 0.5f);
        int startY = Mathf.RoundToInt(room.worldPosition.y - room.size.y * 0.5f);

        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
                SetCell(new Vector2Int(startX + x, startY + y), CellType.Floor);
        }
    }

    private void MarkCorridorFloor(CorridorData corridor)
    {
        int width = Mathf.Max(1, Mathf.RoundToInt(corridor.width));
        int halfWidth = width / 2;
        int minOffset = -(width - 1) / 2;
        int maxOffset = width / 2;

        for (int i = 0; i < corridor.points.Length - 1; i++)
        {
            Vector2 from = corridor.points[i];
            Vector2 to = corridor.points[i + 1];

            bool isHorizontal = Mathf.Abs(to.y - from.y) < 0.1f;

            int startX = Mathf.RoundToInt(Mathf.Min(from.x, to.x));
            int startY = Mathf.RoundToInt(Mathf.Min(from.y, to.y));
            int endX = Mathf.RoundToInt(Mathf.Max(from.x, to.x));
            int endY = Mathf.RoundToInt(Mathf.Max(from.y, to.y));

            if (isHorizontal)
            {
                for (int x = startX; x <= endX; x++)
                {
                    for (int w = minOffset; w <= maxOffset; w++)
                        SetCell(new Vector2Int(x, startY + w), CellType.Floor);
                }
            }
            else
            {
                for (int y = startY; y <= endY; y++)
                {
                    for (int w = minOffset; w <= maxOffset; w++)
                        SetCell(new Vector2Int(startX + w, y), CellType.Floor);
                }
            }

            // 꺾이는 지점 메움
            if (i < corridor.points.Length - 2)
            {
                int cx = Mathf.RoundToInt(corridor.points[i + 1].x);
                int cy = Mathf.RoundToInt(corridor.points[i + 1].y);

                for (int dx = minOffset; dx <= maxOffset; dx++)
                {
                    for (int dy = minOffset; dy <= maxOffset; dy++)
                        SetCell(new Vector2Int(cx + dx, cy + dy), CellType.Floor);
                }
            }
        }
    }

    private void MarkDoors(RoomNode room)
    {
        foreach (DoorData door in room.doors)
        {
            Vector2 worldPos = room.worldPosition + door.localPosition;
            int dx = Mathf.RoundToInt(worldPos.x);
            int dy = Mathf.RoundToInt(worldPos.y);

            // 문 자리는 Door로 유지
            for (int i = -2; i <= 2; i++)
            {
                switch (door.direction)
                {
                    case DoorDirection.Up:
                    case DoorDirection.Down:
                        SetCell(new Vector2Int(dx + i, dy), CellType.Door);
                        break;

                    case DoorDirection.Left:
                    case DoorDirection.Right:
                        SetCell(new Vector2Int(dx, dy + i), CellType.Door);
                        break;
                }
            }
        }
    }

    // Door는 덮어쓰지 않음
    private void SetCell(Vector2Int pos, CellType type)
    {
        if (_grid.TryGetValue(pos, out CellType existing))
        {
            if (existing == CellType.Door)
                return;
        }

        _grid[pos] = type;
    }

    // 지정 거리 범위 좌표 반환
    private IEnumerable<Vector2Int> GetNeighbors(Vector2Int pos, int dist)
    {
        for (int dx = -dist; dx <= dist; dx++)
        {
            for (int dy = -dist; dy <= dist; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                yield return new Vector2Int(pos.x + dx, pos.y + dy);
            }
        }
    }

    private void PlaceTile(GameObject prefab, Vector2Int pos)
    {
        if (prefab == null)
            return;

        GameObject tile = Instantiate(
            prefab,
            new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f),
            Quaternion.identity
        );

        tile.transform.SetParent(transform);
        _tiles.Add(tile);
    }

    public void Clear()
    {
        foreach (GameObject tile in _tiles)
        {
            if (tile != null)
                Destroy(tile);
        }

        _tiles.Clear();
        _grid.Clear();
    }
}