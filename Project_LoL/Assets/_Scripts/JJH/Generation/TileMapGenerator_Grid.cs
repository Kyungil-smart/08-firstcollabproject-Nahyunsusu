using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator_Grid : MonoBehaviour
{
    [SerializeField] private MapObjectPool _objectPool;
    [SerializeField] private GameObject _floorPrefab;
    [SerializeField] private GameObject _corridorPrefab;
    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private Transform _tileRoot;

    public Transform TileRoot => _tileRoot;

    private Dictionary<Vector2Int, GameObject> _activeTiles = new Dictionary<Vector2Int, GameObject>();
    private HashSet<Vector2Int> _doorWallPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> _bossRoomCells = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> _corridorTiles = new HashSet<Vector2Int>();

    public void Clear()
    {
        _activeTiles.Clear();
        _doorWallPositions.Clear();
        _bossRoomCells.Clear();
        _corridorTiles.Clear();
    }

    public bool IsCorridor(Vector2Int pos) => _corridorTiles.Contains(pos);

    public void GenerateRoom(RoomNode room)
    {
        RectInt bounds = room.GetBounds();

        if (room.roomData.roomType == RoomType.Boss)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            for (int y = bounds.yMin; y < bounds.yMax; y++)
                _bossRoomCells.Add(new Vector2Int(x, y));

            return;
        }

        if (room.roomData.roomType == RoomType.Start)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            for (int y = bounds.yMin; y < bounds.yMax; y++)
                _activeTiles[new Vector2Int(x, y)] = null;

            return;
        }

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        for (int y = bounds.yMin; y < bounds.yMax; y++)
            PlaceTile(_floorPrefab, new Vector2Int(x, y));
    }

    public void GenerateCorridors(List<ConnectionResult> connections)
    {
        foreach (var conn in connections)
        {
            if (conn.corridorTiles == null || conn.corridorTiles.Count == 0)
                continue;

            int width = Mathf.Max(1, conn.corridorWidth);
            var tiles = conn.corridorTiles;
            int last = tiles.Count - 1;

            bool doorAVertical = conn.doorA.dir == DoorDir.Up || conn.doorA.dir == DoorDir.Down;
            bool doorBVertical = conn.doorB.dir == DoorDir.Up || conn.doorB.dir == DoorDir.Down;

            for (int idx = 0; idx < tiles.Count; idx++)
            {
                Vector2Int pos = tiles[idx];

                bool isVertical;
                if (idx == 0)
                    isVertical = doorAVertical;
                else if (idx == last)
                    isVertical = doorBVertical;
                else
                    isVertical = GetTileDirection(tiles, idx);

                for (int i = 0; i < width; i++)
                {
                    Vector2Int tile = isVertical
                        ? new Vector2Int(pos.x + i, pos.y)
                        : new Vector2Int(pos.x, pos.y + i);

                    PlaceTile(_corridorPrefab, tile);
                }
            }

            RegisterDoorWallPositions(conn.doorA, conn.corridorWidth, doorAVertical);
            RegisterDoorWallPositions(conn.doorB, conn.corridorWidth, doorBVertical);
        }
    }

    private bool GetTileDirection(List<Vector2Int> tiles, int idx)
    {
        if (tiles.Count < 2)
            return true;

        Vector2Int cur = tiles[idx];

        if (idx < tiles.Count - 1)
        {
            Vector2Int next = tiles[idx + 1];
            int dx = Mathf.Abs(next.x - cur.x);
            int dy = Mathf.Abs(next.y - cur.y);
            if (dx != dy)
                return dy > dx;
        }

        if (idx > 0)
        {
            Vector2Int prev = tiles[idx - 1];
            int dx = Mathf.Abs(cur.x - prev.x);
            int dy = Mathf.Abs(cur.y - prev.y);
            if (dx != dy)
                return dy > dx;
        }

        return true;
    }

    private void RegisterDoorWallPositions(DoorCandidate door, int width, bool isVertical)
    {
        for (int i = 0; i < width; i++)
        {
            Vector2Int pos = isVertical
                ? new Vector2Int(door.tileWallPos.x + i, door.tileWallPos.y)
                : new Vector2Int(door.tileWallPos.x, door.tileWallPos.y + i);

            _doorWallPositions.Add(pos);
        }
    }

    public void FinalizeWalls()
    {
        var candidates = new HashSet<Vector2Int>();

        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1)
        };

        foreach (var pos in _activeTiles.Keys)
        {
            foreach (var dir in dirs)
            {
                Vector2Int check = pos + dir;
                if (!_activeTiles.ContainsKey(check))
                    candidates.Add(check);
            }
        }

        foreach (var pos in candidates)
        {
            if (_doorWallPositions.Contains(pos)) continue;
            if (_bossRoomCells.Contains(pos)) continue;
            PlaceTile(_wallPrefab, pos);
        }
    }

    public List<Vector2Int> GetFloorPositionsInRoom(RoomNode room)
    {
        var result = new List<Vector2Int>();
        RectInt bounds = room.GetBounds();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            Vector2Int pos = new Vector2Int(x, y);
            if (_activeTiles.ContainsKey(pos))
                result.Add(pos);
        }

        return result;
    }

    private void PlaceTile(GameObject prefab, Vector2Int pos)
    {
        if (prefab == null) return;

        if (_activeTiles.TryGetValue(pos, out var existing) && existing != null) return;

        GameObject tile = _objectPool.Spawn(
            prefab,
            _tileRoot,
            new Vector3(pos.x, pos.y, 0f),
            Quaternion.identity
        );

        _activeTiles[pos] = tile;

        if (prefab == _corridorPrefab)
            _corridorTiles.Add(pos);
    }
}