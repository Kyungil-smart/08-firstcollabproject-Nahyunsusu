using System.Collections.Generic;
using UnityEngine;
 
public class MapManager : MonoBehaviour
{
    [SerializeField] private MapRoomPool _roomPool;
    [SerializeField] private ConnectionPlanner _planner;
    [SerializeField] private TileMapGenerator_Grid _tileGenerator;
    [SerializeField] private DoorController _doorController;
    [SerializeField] private MapObjectPool _objectPool;
    [SerializeField] private int _roomSpacing = 30;
 
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
        _objectPool.ReleaseChildren(_tileGenerator.TileRoot);
        _tileGenerator.Clear();
        _runtimeDataMap.Clear();
    }
 
    private void PlaceRooms()
    {
        foreach (var room in _graph.allRooms)
        {
            if (room.roomData.floorPrefab != null)
            {
                if (room.roomData.roomType == RoomType.Start)
                {
                    _objectPool.Spawn(room.roomData.floorPrefab, _tileGenerator.TileRoot, Vector3.zero, Quaternion.identity);
                }
                else
                {
                    Vector3 pos = new Vector3(
                        room.gridOrigin.x + room.size.x * 0.5f,
                        room.gridOrigin.y + room.size.y * 0.5f,
                        0f
                    );
                    _objectPool.Spawn(room.roomData.floorPrefab, _tileGenerator.TileRoot, pos, Quaternion.identity);
                }
            }
 
            _tileGenerator.GenerateRoom(room);
 
            if (room.roomData.roomType == RoomType.Combat)
                PlaceRoomTrigger(room);
        }
    }
 
    private void PlaceRoomTrigger(RoomNode room)
    {
        RectInt b = room.GetBounds();
 
        GameObject triggerObj = new GameObject("RoomTrigger");
        triggerObj.transform.SetParent(_tileGenerator.TileRoot);
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
    }
 
    private void PlaceConnections(List<ConnectionResult> connections)
    {
        _tileGenerator.GenerateCorridors(connections);
        _tileGenerator.FinalizeWalls();
        _doorController.BuildDoors(connections);
    }
}