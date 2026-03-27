using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private MapRoomPool _roomPool; 
    [SerializeField] private ConnectionPlanner _planner;
    [SerializeField] private TileMapGenerator_Grid _tileGenerator;
    [SerializeField] private DoorController _doorController;
    [SerializeField] private MapObjectPool _objectPool;
    [SerializeField] private int _roomSpacing = 20;

    private MapGraph _graph;
    private Dictionary<RoomNode, RoomRuntimeData> _runtimeDataMap = new Dictionary<RoomNode, RoomRuntimeData>();

    private void Start()
    {
        if (_roomPool != null) BuildMap(_roomPool);
    }

    public void BuildMap(MapRoomPool pool)
    {
#if UNITY_EDITOR
        UnityEditor.Selection.activeGameObject = null;
#endif
        if (pool == null) return;

        _objectPool.ReleaseChildren(_tileGenerator.TileRoot);
        _tileGenerator.Clear();
        _runtimeDataMap.Clear();

        _graph = new MapGraph();
        _graph.Generate(pool, _roomSpacing);
        
        if (_graph.allRooms.Count == 0) return;

        List<ConnectionResult> connections = _planner.PlanAll(_graph);

        foreach (var room in _graph.allRooms)
        {
            // 1. 런타임 데이터 미리 생성
            RoomRuntimeData runtimeData = GetRuntimeData(room);
            GameObject roomInstance = null;

            if (room.roomData.roomType == RoomType.Boss || room.roomData.roomType == RoomType.Start)
            {
                if (room.roomData.floorPrefab != null)
                {
                    roomInstance = _objectPool.Spawn(room.roomData.floorPrefab, _tileGenerator.TileRoot, (Vector3)room.worldPosition, Quaternion.identity);
                    if (room.roomData.roomType == RoomType.Start) _tileGenerator.RegisterExistFloor(room);
                }
            }
            else
            {
                // 일반 방은 타일 생성 (필요 시 타일 루트를 인스턴스로 취급)
                _tileGenerator.GenerateRoom(room);
            }

            // 2. [에러 방지] RoomTrigger 및 관련 컴포넌트에 데이터 주입
            if (roomInstance != null)
            {
                // RoomTrigger가 있다면 데이터를 즉시 넣어줌 (Null 에러 방지)
                var trigger = roomInstance.GetComponentInChildren<RoomTrigger>();
                if (trigger != null)
                {
                    // 작성하신 RoomTrigger의 초기화 메서드 이름에 맞춰 수정하세요 (예: SetRoomData)
                    // trigger.SetRoomData(runtimeData); 
                }
            }
        }

        _tileGenerator.GenerateCorridors(connections);
        _tileGenerator.FinalizeWalls();
        _doorController.BuildDoors(connections);
        
        // 3. 모든 생성이 끝난 후 다른 매니저들에게 알림 (필요 시)
        // FindObjectOfType<RoomClearManager>()?.RefreshRooms();
    }

    public RoomRuntimeData GetRuntimeData(RoomNode room)
    {
        if (room == null) return null;
        if (!_runtimeDataMap.TryGetValue(room, out var data))
        {
            data = new RoomRuntimeData(room);
            _runtimeDataMap[room] = data;
        }
        return data;
    }
}