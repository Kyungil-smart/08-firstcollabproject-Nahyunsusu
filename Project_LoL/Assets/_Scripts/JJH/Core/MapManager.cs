using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private MapRoomPool _roomPool;
    [SerializeField] private ConnectionPlanner _planner;
    [SerializeField] private TileMapGenerator_Grid _tileGenerator;
    [SerializeField] private DoorController _doorController;
    [SerializeField] private MapObjectPool _objectPool;
    [SerializeField] private Transform _spawnedPrefabRoot;
    [SerializeField] private int _roomSpacing = 30;
    [SerializeField] private MiniMapUI _miniMapUI;

    private MapGraph _graph;
    private Dictionary<RoomNode, RoomRuntimeData> _runtimeDataMap = new Dictionary<RoomNode, RoomRuntimeData>();

    private void Start()
    {
        if (_roomPool == null) return;
        BuildMap(_roomPool);
    }

    public void BuildMap(MapRoomPool pool)
    {
        if (pool == null) return;
        if (_planner == null || _tileGenerator == null || _doorController == null || _objectPool == null) return;

        ClearMap();

        _graph = new MapGraph();
        _graph.Generate(pool, _roomSpacing);

        foreach (var room in _graph.allRooms)
            _runtimeDataMap[room] = new RoomRuntimeData(room);

        List<ConnectionResult> connections = _planner.PlanAll(_graph);

        PlaceRooms();
        PlaceConnections(connections);

        if (_miniMapUI != null)
        {
            _miniMapUI.BuildMiniMap(_graph.allRooms);
            _miniMapUI.UpdateMiniMap(_graph.startRoom);
        }
    }

    public RoomRuntimeData GetRuntimeData(RoomNode room)
    {
        _runtimeDataMap.TryGetValue(room, out var data);
        return data;
    }

    public void OnCombatCleared(RoomNode room)
    {
        _doorController.OpenDoors(room);
    }

    private void ClearMap()
    {
        _tileGenerator.Clear();
        _runtimeDataMap.Clear();

        if (_spawnedPrefabRoot != null)
            _objectPool.ReleaseChildren(_spawnedPrefabRoot);
    }

    private void PlaceRooms()
    {
        foreach (var room in _graph.allRooms)
        {
            if (room.roomData.floorPrefab != null)
            {
                if (room.roomData.roomType == RoomType.Start)
                {
                    _objectPool.Spawn(room.roomData.floorPrefab, _spawnedPrefabRoot, Vector3.zero, Quaternion.identity);
                }
                else if (room.roomData.roomType == RoomType.Boss)
                {
                    Vector3 pos = new Vector3(
                        room.gridOrigin.x + room.size.x * 0.5f,
                        room.gridOrigin.y + room.size.y * 0.5f - 1.0f,
                        0f
                    );
                    _objectPool.Spawn(room.roomData.floorPrefab, _spawnedPrefabRoot, pos, Quaternion.identity);
                }
                else
                {
                    Vector3 pos = new Vector3(
                        room.gridOrigin.x + room.size.x * 0.5f,
                        room.gridOrigin.y + room.size.y * 0.5f,
                        0f
                    );
                    _objectPool.Spawn(room.roomData.floorPrefab, _spawnedPrefabRoot, pos, Quaternion.identity);
                }
            }

            _tileGenerator.GenerateRoom(room);

            if (room.roomData.roomType == RoomType.Combat)
                PlaceRoomTrigger(room);

            if (room.roomData.roomType == RoomType.Boss)
                PlaceBossTrigger(room);
        }
    }

    private void PlaceRoomTrigger(RoomNode room)
    {
        RectInt b = room.GetBounds();

        GameObject triggerObj = new GameObject("RoomTrigger");
        triggerObj.transform.SetParent(_spawnedPrefabRoot);
        triggerObj.transform.position = new Vector3(
            b.xMin + b.width / 2f,
            b.yMin + b.height / 2f,
            0f
        );

        BoxCollider2D col = triggerObj.AddComponent<BoxCollider2D>();
        col.size = new Vector2(b.width, b.height);
        col.isTrigger = true;

        RoomTrigger rt = triggerObj.AddComponent<RoomTrigger>();
        rt.room = room;
        rt.mapManager = this;
        rt.doorController = _doorController;
        rt.miniMapUI = _miniMapUI;
    }

    private void PlaceBossTrigger(RoomNode room)
    {
        RectInt b = room.GetBounds();

        GameObject triggerObj = new GameObject("BossTrigger");
        triggerObj.transform.SetParent(_spawnedPrefabRoot);
        triggerObj.transform.position = new Vector3(
            b.xMin + b.width / 2f,
            b.yMin + 1f,
            0f
        );

        BoxCollider2D col = triggerObj.AddComponent<BoxCollider2D>();
        col.size = new Vector2(b.width, 2f);
        col.isTrigger = true;

        RoomTrigger rt = triggerObj.AddComponent<RoomTrigger>();
        rt.room = room;
        rt.mapManager = this;
        rt.doorController = _doorController;
        rt.miniMapUI = _miniMapUI;
    }

    private void PlaceConnections(List<ConnectionResult> connections)
    {
        _tileGenerator.GenerateCorridors(connections);
        _tileGenerator.FinalizeWalls();

        List<DoorPlacement> placements = BuildDoorPlacements(connections);
        _doorController.BuildDoors(placements);
    }

    private List<DoorPlacement> BuildDoorPlacements(List<ConnectionResult> connections)
    {
        var best = new Dictionary<(string, DoorDir, Vector2Int), DoorPlacement>();

        foreach (var conn in connections)
        {
            TryAddPlacement(best, conn.roomA, conn.doorA.dir, conn.doorA.tileWallPos, conn.corridorWidth);
            TryAddPlacement(best, conn.roomB, conn.doorB.dir, conn.doorB.tileWallPos, conn.corridorWidth);
        }

        return new List<DoorPlacement>(best.Values);
    }

    private void TryAddPlacement(
        Dictionary<(string, DoorDir, Vector2Int), DoorPlacement> best,
        RoomNode room, DoorDir dir, Vector2Int tileWallPos, int width)
    {
        var key = (room.nodeId, dir, tileWallPos);

        if (!best.TryGetValue(key, out var existing) || width > existing.width)
        {
            best[key] = new DoorPlacement
            {
                owner = room,
                dir = dir,
                pivot = tileWallPos,
                width = width
            };
        }
    }
}