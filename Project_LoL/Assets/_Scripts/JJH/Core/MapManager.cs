using System.Collections.Generic;
using UnityEngine;
 
public class MapManager : MonoBehaviour
{
    [SerializeField] private MapRoomPool _roomPool;
    [SerializeField] private ConnectionPlanner _planner;
    [SerializeField] private TileMapGenerator_Grid _tileGenerator;
    [SerializeField] private DoorController _doorController;
    [SerializeField] private MapObjectPool _objectPool;
    [SerializeField] private int _roomSpacing = 15;
 
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
 
    // 전투 클리어 시 RoomClearManager에서 호출
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
        }
    }
 
    private void PlaceConnections(List<ConnectionResult> connections)
    {
        _tileGenerator.GenerateCorridors(connections);
        _tileGenerator.FinalizeWalls();
        _doorController.BuildDoors(connections);
    }
}