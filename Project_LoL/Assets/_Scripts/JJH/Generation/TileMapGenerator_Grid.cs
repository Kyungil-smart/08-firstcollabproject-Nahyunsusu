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
 
    public void Clear()
    {
        _activeTiles.Clear();
        _doorWallPositions.Clear();
    }
    
    public void GenerateRoom(RoomNode room)
    {
        if (room.roomData.roomType == RoomType.Boss) return;
 
        Vector2Int origin = room.gridOrigin;
        for (int x = 0; x < room.size.x; x++)
            for (int y = 0; y < room.size.y; y++)
                PlaceTile(_floorPrefab, new Vector2Int(origin.x + x, origin.y + y));
    }
 
    public void GenerateCorridors(List<ConnectionResult> connections)
    {
        foreach (var conn in connections)
        {
            _doorWallPositions.Add(conn.doorA.wallPos);
            _doorWallPositions.Add(conn.doorB.wallPos);
 
            if (conn.corridorTiles == null) continue;
            foreach (var pos in conn.corridorTiles)
                PlaceTile(_corridorPrefab, pos);
        }
    }
    
    public void FinalizeWalls()
    {
        var wallCandidates = new HashSet<Vector2Int>();
 
        foreach (var pos in _activeTiles.Keys)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    Vector2Int check = pos + new Vector2Int(dx, dy);
                    if (!_activeTiles.ContainsKey(check))
                        wallCandidates.Add(check);
                }
            }
        }
 
        foreach (var w in wallCandidates)
        {
            if (_doorWallPositions.Contains(w)) continue;
            PlaceTile(_wallPrefab, w);
        }
    }
 
    public List<Vector2Int> GetFloorPositionsInRoom(RoomNode room)
    {
        var result = new List<Vector2Int>();
        RectInt b = room.GetBounds();
 
        for (int x = b.xMin; x < b.xMax; x++)
            for (int y = b.yMin; y < b.yMax; y++)
            {
                var pos = new Vector2Int(x, y);
                if (_activeTiles.ContainsKey(pos))
                    result.Add(pos);
            }
 
        return result;
    }
 
    private void PlaceTile(GameObject prefab, Vector2Int pos)
    {
        if (prefab == null) return;
        if (_activeTiles.ContainsKey(pos)) return;
 
        GameObject t = _objectPool.Spawn(
            prefab, _tileRoot,
            new Vector3(pos.x, pos.y, 0f),
            Quaternion.identity
        );
        _activeTiles[pos] = t;
    }
}