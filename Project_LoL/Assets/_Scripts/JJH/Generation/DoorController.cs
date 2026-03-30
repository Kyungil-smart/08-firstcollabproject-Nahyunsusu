using System.Collections.Generic;
using UnityEngine;
 
public class DoorController : MonoBehaviour
{
    [SerializeField] private MapObjectPool _objectPool;
    [SerializeField] private GameObject _doorPrefab;
    [SerializeField] private Transform _doorRoot;
 
    private Dictionary<RoomNode, List<DoorObject>> _roomDoors = new Dictionary<RoomNode, List<DoorObject>>();
 
    public void BuildDoors(List<ConnectionResult> connections)
    {
        _objectPool.ReleaseChildren(_doorRoot);
        _roomDoors.Clear();
 
        foreach (var conn in connections)
        {
            PlaceDoor(conn.doorA, conn.corridorWidth);
 
            if (conn.doorB.owner != null &&
                conn.doorB.owner.roomData.roomType != RoomType.Boss)
                PlaceDoor(conn.doorB, conn.corridorWidth);
        }
    }
 
    private void PlaceDoor(DoorCandidate door, int width)
    {
        if (door == null) return;
 
        bool isVertical = door.dir == DoorDir.Up || door.dir == DoorDir.Down;
        float centerOffset = (width - 1) / 2f;
 
        Vector3 worldPos = isVertical
            ? new Vector3(door.wallPos.x + centerOffset, door.wallPos.y, 0f)
            : new Vector3(door.wallPos.x, door.wallPos.y + centerOffset, 0f);
 
        GameObject d = _objectPool.Spawn(_doorPrefab, _doorRoot, worldPos, Quaternion.identity);
 
        float angle = door.dir switch
        {
            DoorDir.Up    => 0f,
            DoorDir.Down  => 180f,
            DoorDir.Left  => 90f,
            _             => -90f
        };
        d.transform.rotation = Quaternion.Euler(0f, 0f, angle);
 
        DoorObject doorObj = d.GetComponent<DoorObject>();
        if (doorObj != null)
            doorObj.Setup(width, isVertical);
 
        if (door.owner != null)
        {
            if (!_roomDoors.ContainsKey(door.owner))
                _roomDoors[door.owner] = new List<DoorObject>();
 
            if (doorObj != null)
                _roomDoors[door.owner].Add(doorObj);
        }
    }
 
    public void CloseDoors(RoomNode room)
    {
        if (!_roomDoors.TryGetValue(room, out var doors)) return;
        foreach (var d in doors)
            if (d != null) d.Close();
    }
 
    public void OpenDoors(RoomNode room)
    {
        if (!_roomDoors.TryGetValue(room, out var doors)) return;
        foreach (var d in doors)
            if (d != null) d.Open();
    }
 
    public List<Vector3> GetDoorWorldPositions(RoomNode room)
    {
        var result = new List<Vector3>();
        if (!_roomDoors.TryGetValue(room, out var doors)) return result;
        foreach (var d in doors)
            if (d != null) result.Add(d.transform.position);
        return result;
    }
}