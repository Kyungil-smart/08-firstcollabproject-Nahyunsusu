using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator_Grid : MonoBehaviour
{
    public enum CellType { Empty, Floor, CorridorFloor, Door }

    [Header("타일 프리팹")]
    public GameObject floorTilePrefab;
    public GameObject corridorFloorTilePrefab;
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

        // Floor/CorridorFloor/Door만 따로 모아서 기준으로 사용
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, CellType> cell in _grid)
        {
            if (cell.Value == CellType.Floor ||
                cell.Value == CellType.CorridorFloor ||
                cell.Value == CellType.Door)
                floorPositions.Add(cell.Key);
        }

        // 바닥 기준 외곽 1칸 벽 생성
        HashSet<Vector2Int> wallSet = new HashSet<Vector2Int>();
        foreach (Vector2Int pos in floorPositions)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    Vector2Int neighbor = pos + new Vector2Int(dx, dy);
                    if (!floorPositions.Contains(neighbor))
                        wallSet.Add(neighbor);
                }
            }
        }

        // wallThickness만큼 레이어 확장
        for (int w = 2; w <= wallThickness; w++)
        {
            HashSet<Vector2Int> nextLayer = new HashSet<Vector2Int>();
            foreach (Vector2Int pos in wallSet)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        Vector2Int neighbor = pos + new Vector2Int(dx, dy);
                        if (!floorPositions.Contains(neighbor) && !wallSet.Contains(neighbor))
                            nextLayer.Add(neighbor);
                    }
                }
            }
            wallSet.UnionWith(nextLayer);
        }

        // 타입별로 다른 프리팹으로 생성
        foreach (KeyValuePair<Vector2Int, CellType> cell in _grid)
        {
            if (cell.Value == CellType.Floor || cell.Value == CellType.Door)
                PlaceTile(floorTilePrefab, cell.Key);
            else if (cell.Value == CellType.CorridorFloor)
                PlaceTile(corridorFloorTilePrefab, cell.Key);
        }

        foreach (Vector2Int pos in wallSet)
            PlaceTile(wallTilePrefab, pos);
    }

    private void MarkRoomFloor(RoomNode room)
    {
        int startX = Mathf.RoundToInt(room.worldPosition.x - room.size.x * 0.5f);
        int startY = Mathf.RoundToInt(room.worldPosition.y - room.size.y * 0.5f);

        for (int x = 0; x < room.size.x; x++)
            for (int y = 0; y < room.size.y; y++)
                SetCell(new Vector2Int(startX + x, startY + y), CellType.Floor);
    }

    private void MarkCorridorFloor(CorridorData corridor)
    {
        int width = Mathf.Max(1, Mathf.RoundToInt(corridor.width));
        int minOffset = -(width - 1) / 2;
        int maxOffset = width / 2;

        for (int i = 0; i < corridor.points.Length - 1; i++)
        {
            Vector2 from = corridor.points[i];
            Vector2 to   = corridor.points[i + 1];

            bool isHorizontal = Mathf.Abs(to.y - from.y) < 0.1f;

            int startX = Mathf.RoundToInt(Mathf.Min(from.x, to.x));
            int startY = Mathf.RoundToInt(Mathf.Min(from.y, to.y));
            int endX   = Mathf.RoundToInt(Mathf.Max(from.x, to.x));
            int endY   = Mathf.RoundToInt(Mathf.Max(from.y, to.y));

            if (isHorizontal)
            {
                for (int x = startX; x <= endX; x++)
                    for (int w = minOffset; w <= maxOffset; w++)
                        SetCell(new Vector2Int(x, startY + w), CellType.CorridorFloor);
            }
            else
            {
                for (int y = startY; y <= endY; y++)
                    for (int w = minOffset; w <= maxOffset; w++)
                        SetCell(new Vector2Int(startX + w, y), CellType.CorridorFloor);
            }

            // 꺾이는 지점 메움
            if (i < corridor.points.Length - 2)
            {
                int cx = Mathf.RoundToInt(corridor.points[i + 1].x);
                int cy = Mathf.RoundToInt(corridor.points[i + 1].y);

                for (int dx = minOffset; dx <= maxOffset; dx++)
                    for (int dy = minOffset; dy <= maxOffset; dy++)
                        SetCell(new Vector2Int(cx + dx, cy + dy), CellType.CorridorFloor);
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

                        // 문 바깥 방향 두 칸은 Floor로 보호
                        int outerY1 = dy + (door.direction == DoorDirection.Up ? 1 : -1);
                        int outerY2 = dy + (door.direction == DoorDirection.Up ? 2 : -2);

                        SetCell(new Vector2Int(dx + i, outerY1), CellType.Floor);
                        SetCell(new Vector2Int(dx + i, outerY2), CellType.Floor);
                        break;

                    case DoorDirection.Left:
                    case DoorDirection.Right:
                        SetCell(new Vector2Int(dx, dy + i), CellType.Door);

                        // 문 바깥 방향 두 칸은 Floor로 보호
                        int outerX1 = dx + (door.direction == DoorDirection.Right ? 1 : -1);
                        int outerX2 = dx + (door.direction == DoorDirection.Right ? 2 : -2);

                        SetCell(new Vector2Int(outerX1, dy + i), CellType.Floor);
                        SetCell(new Vector2Int(outerX2, dy + i), CellType.Floor);
                        break;
                }
            }
        }
    }

    // Door > Floor > CorridorFloor 우선순위
    private void SetCell(Vector2Int pos, CellType type)
    {
        if (_grid.TryGetValue(pos, out CellType existing))
        {
            if (existing == CellType.Door)
                return;

            if (existing == CellType.Floor && type == CellType.CorridorFloor)
                return;
        }

        _grid[pos] = type;
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