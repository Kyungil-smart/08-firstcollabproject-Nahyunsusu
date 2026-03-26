using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator_Grid : MonoBehaviour
{
    public enum CellType { Empty, Floor, CorridorFloor }

    [Header("타일 프리팹")]
    public GameObject floorTilePrefab;
    public GameObject corridorFloorTilePrefab;
    public GameObject wallTilePrefab;

    [Header("벽 설정")]
    public int wallThickness = 2;

    private Dictionary<Vector2Int, CellType> _grid = new Dictionary<Vector2Int, CellType>();
    private HashSet<Vector2Int> _doorPositions = new HashSet<Vector2Int>();
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

        // 복도 중간에 비는 칸 보정
        FillCorridorGaps();

        // 문 위치 등록
        foreach (RoomNode room in graph.allRooms)
            MarkDoors(room);

        // Floor/CorridorFloor만 floorPositions으로 추출
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, CellType> cell in _grid)
        {
            if (cell.Value == CellType.Floor || cell.Value == CellType.CorridorFloor)
                floorPositions.Add(cell.Key);
        }

        // 문 좌표도 벽 생성 기준에 포함
        foreach (Vector2Int doorPos in _doorPositions)
            floorPositions.Add(doorPos);

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

        // 타입에 맞는 타일 생성
        foreach (KeyValuePair<Vector2Int, CellType> cell in _grid)
        {
            if (cell.Value == CellType.Floor)
                PlaceTile(floorTilePrefab, cell.Key);
            else if (cell.Value == CellType.CorridorFloor)
                PlaceTile(corridorFloorTilePrefab, cell.Key);
        }

        // 벽 생성 시 문 좌표는 제외
        foreach (Vector2Int pos in wallSet)
        {
            if (_doorPositions.Contains(pos))
                continue;

            PlaceTile(wallTilePrefab, pos);
        }
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

    // 복도 중간에 비는 칸 보정
    private void FillCorridorGaps()
    {
        HashSet<Vector2Int> fillSet = new HashSet<Vector2Int>();

        foreach (KeyValuePair<Vector2Int, CellType> cell in _grid)
        {
            if (cell.Value != CellType.CorridorFloor)
                continue;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    Vector2Int checkPos = cell.Key + new Vector2Int(dx, dy);

                    // 이미 바닥이 있으면 제외
                    if (_grid.TryGetValue(checkPos, out CellType existing) && existing == CellType.Floor)
                        continue;

                    if (_grid.ContainsKey(checkPos))
                        continue;

                    int corridorCount = 0;

                    if (_grid.TryGetValue(
                            checkPos + Vector2Int.up,
                            out var t1) && t1 == CellType.CorridorFloor)
                        corridorCount++;
                    if (_grid.TryGetValue(
                            checkPos + Vector2Int.down,
                            out var t2) && t2 == CellType.CorridorFloor)
                        corridorCount++;
                    if (_grid.TryGetValue(
                            checkPos + Vector2Int.left,
                            out var t3) && t3 == CellType.CorridorFloor)
                        corridorCount++;
                    if (_grid.TryGetValue(
                            checkPos + Vector2Int.right,
                            out var t4) && t4 == CellType.CorridorFloor)
                        corridorCount++;

                    // 상하좌우로 복도에 둘러싸인 칸만 메움
                    if (corridorCount >= 2)
                        fillSet.Add(checkPos);
                }
            }
        }

        foreach (Vector2Int pos in fillSet)
        {
            if (_grid.TryGetValue(pos, out CellType existing) && existing == CellType.Floor)
                continue;

            SetCell(pos, CellType.CorridorFloor);
        }
    }
    
    // 문 위치 등록
    private void MarkDoors(RoomNode room)
    {
        foreach (DoorData door in room.doors)
        {
            Vector2 worldPos = room.worldPosition + door.localPosition;
            int dx = Mathf.RoundToInt(worldPos.x);
            int dy = Mathf.RoundToInt(worldPos.y);

            int width = Mathf.Max(1, door.openingWidth);
            int minOffset = -(width - 1) / 2;
            int maxOffset = width / 2;

            for (int i = minOffset; i <= maxOffset; i++)
            {
                switch (door.direction)
                {
                    case DoorDirection.Up:
                    case DoorDirection.Down:
                        _doorPositions.Add(new Vector2Int(dx + i, dy));
                        break;

                    case DoorDirection.Left:
                    case DoorDirection.Right:
                        _doorPositions.Add(new Vector2Int(dx, dy + i));
                        break;
                }
            }
        }
    }

    // Room Floor는 CorridorFloor에 덮이지 않음
    private void SetCell(Vector2Int pos, CellType type)
    {
        if (_grid.TryGetValue(pos, out CellType existing))
        {
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
    
    // 특정 좌표가 바닥 타일인지 확인 (스폰 가능 여부 판단용)
    public bool IsFloor(Vector2Int pos)
    {
        return _grid.TryGetValue(pos, out CellType type) &&
               (type == CellType.Floor || type == CellType.CorridorFloor);
    }

    // 방 내부 바닥 좌표 목록 반환 (스폰 후보 위치 계산용)
    public List<Vector2Int> GetFloorPositionsInRoom(RoomNode room)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        int startX = Mathf.RoundToInt(room.worldPosition.x - room.size.x * 0.5f);
        int startY = Mathf.RoundToInt(room.worldPosition.y - room.size.y * 0.5f);

        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
            {
                Vector2Int pos = new Vector2Int(startX + x, startY + y);

                if (IsFloor(pos))
                    result.Add(pos);
            }
        }

        return result;
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
        _doorPositions.Clear();
    }
}