using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator_Grid : MonoBehaviour
{
    [Header("오브젝트 풀 참조")]
    [SerializeField] private MapObjectPool _objectPool;

    [Header("타일 프리팹")]
    [SerializeField] private GameObject _floorPrefab;
    [SerializeField] private GameObject _corridorPrefab;
    [SerializeField] private GameObject _wallPrefab;

    [Header("생성 루트")]
    [SerializeField] private Transform _tileRoot;
    public Transform TileRoot => _tileRoot;

    private Dictionary<Vector2Int, GameObject> _activeTiles = new Dictionary<Vector2Int, GameObject>();

    public void Clear() => _activeTiles.Clear();

    public void RegisterExistFloor(RoomNode room)
    {
        Vector2Int size = room.size;
        Vector2Int startPos = Vector2Int.FloorToInt(room.worldPosition);

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int pos = new Vector2Int(startPos.x + x, startPos.y + y);
                if (!_activeTiles.ContainsKey(pos))
                    _activeTiles.Add(pos, null);
            }
        }
    }

    public void GenerateRoom(RoomNode room)
    {
        Vector2Int size = room.size;
        Vector2Int startPos = Vector2Int.FloorToInt(room.worldPosition);

        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                SpawnTile(_floorPrefab, new Vector2Int(startPos.x + x, startPos.y + y));
    }

    public void GenerateCorridors(List<ConnectionResult> connections)
    {
        foreach (var conn in connections)
            foreach (var pos in conn.corridorPoints)
                SpawnTile(_corridorPrefab, pos);
    }

    public void FinalizeWalls()
    {
        HashSet<Vector2Int> wallCandidates = new HashSet<Vector2Int>();
        foreach (var tilePos in _activeTiles.Keys)
        {
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int checkPos = tilePos + new Vector2Int(x, y);
                    if (!_activeTiles.ContainsKey(checkPos)) wallCandidates.Add(checkPos);
                }
        }
        foreach (var wPos in wallCandidates) SpawnTile(_wallPrefab, wPos);
    }

    private void SpawnTile(GameObject prefab, Vector2Int pos)
    {
        if (_activeTiles.ContainsKey(pos) || prefab == null) return;

        Vector3 worldPos = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
        GameObject tile = _objectPool.Spawn(prefab, _tileRoot, worldPos, Quaternion.identity);
        _activeTiles.Add(pos, tile);
    }

    public List<Vector2Int> GetFloorPositionsInRoom(RoomNode room)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        Vector2Int startPos = Vector2Int.FloorToInt(room.worldPosition);

        for (int x = 0; x < room.size.x; x++)
            for (int y = 0; y < room.size.y; y++)
            {
                Vector2Int pos = new Vector2Int(startPos.x + x, startPos.y + y);
                if (_activeTiles.ContainsKey(pos)) result.Add(pos);
            }
        return result;
    }
}