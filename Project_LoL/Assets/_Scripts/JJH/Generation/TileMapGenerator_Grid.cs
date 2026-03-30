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

    public void Clear()
    {
        _activeTiles.Clear();
        _doorWallPositions.Clear();
        _bossRoomCells.Clear();
    }

    public void GenerateRoom(RoomNode room)
    {
        RectInt bounds = room.GetBounds();

        // 보스방은 벽 생성 제외용으로만 기록
        if (room.roomData.roomType == RoomType.Boss)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            for (int y = bounds.yMin; y < bounds.yMax; y++)
                _bossRoomCells.Add(new Vector2Int(x, y));

            return;
        }

        // 시작방은 타일 예약만 (오브젝트 생성 없음)
        if (room.roomData.roomType == RoomType.Start)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            for (int y = bounds.yMin; y < bounds.yMax; y++)
                _activeTiles[new Vector2Int(x, y)] = null;

            return;
        }

        // 일반 방
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

            _doorWallPositions.Add(conn.doorA.wallPos);
            _doorWallPositions.Add(conn.doorB.wallPos);

            foreach (var pos in conn.corridorTiles)
                PlaceTile(_corridorPrefab, pos);
        }
    }

    public void FinalizeWalls()
    {
        var candidates = new HashSet<Vector2Int>();

        // 8방향 명시
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
    }
}