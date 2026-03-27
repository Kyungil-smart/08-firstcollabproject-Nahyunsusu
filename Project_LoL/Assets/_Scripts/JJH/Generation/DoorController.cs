using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("오브젝트 풀 참조")]
    [SerializeField] private MapObjectPool _objectPool;

    [Header("문 설정")]
    [SerializeField] private GameObject _doorPrefab;
    [SerializeField] private Transform _doorRoot;

    private Dictionary<RoomNode, List<GameObject>> _roomDoors = new Dictionary<RoomNode, List<GameObject>>();

    public void BuildDoors(List<ConnectionResult> connections)
    {
        _objectPool.ReleaseChildren(_doorRoot);
        _roomDoors.Clear();

        foreach (var conn in connections)
        {
            PlaceDoor(conn.roomA, conn.doorA);
            PlaceDoor(conn.roomB, conn.doorB);
        }
    }

    private void PlaceDoor(RoomNode owner, DoorCandidate door)
    {
        Vector3 worldPos = new Vector3(door.pos.x + 0.5f, door.pos.y + 0.5f, 0f);

        GameObject doorObj = _objectPool.Spawn(_doorPrefab, _doorRoot, worldPos, Quaternion.identity);

        SetDoorRotation(doorObj, door.dir);

        if (!_roomDoors.ContainsKey(owner))
            _roomDoors[owner] = new List<GameObject>();

        _roomDoors[owner].Add(doorObj);

        SetDoorState(doorObj, false);
    }

    private void SetDoorRotation(GameObject doorObj, DoorDir dir)
    {
        float angle = dir switch
        {
            DoorDir.Up => 0f,
            DoorDir.Down => 180f,
            DoorDir.Left => 90f,
            DoorDir.Right => -90f,
            _ => 0f
        };

        doorObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetDoorState(GameObject door, bool isClosed)
    {
    }

    public void CloseDoorsInRoom(RoomNode room)
    {
        if (_roomDoors.TryGetValue(room, out var doors))
        {
            foreach (var d in doors)
                SetDoorState(d, true);
        }
    }
    
    public void CloseDoors(RoomNode room)
    {
        // 담당자가 짠 RoomTrigger에서 호출하는 이름 그대로 유지
        CloseDoorsInRoom(room);
    }
    
    public List<Vector3> GetDoorWorldPositions(RoomNode room)
    {
        List<Vector3> positions = new List<Vector3>();
        if (_roomDoors.TryGetValue(room, out var doors))
        {
            foreach (var d in doors) positions.Add(d.transform.position);
        }
        return positions;
    }
}