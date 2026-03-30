using System.Collections.Generic;
using UnityEngine;
 
public class DoorController : MonoBehaviour
{
    [SerializeField] private MapObjectPool _objectPool;
    [SerializeField] private GameObject _doorPrefab;
    [SerializeField] private Transform _doorRoot;
    
    private Dictionary<RoomNode, List<GameObject>> _roomDoors = new Dictionary<RoomNode, List<GameObject>>();
 
    public void BuildDoors(List<ConnectionResult> connections)
    {
        _objectPool.ReleaseChildren(_doorRoot);
        _roomDoors.Clear();
 
        foreach (var conn in connections)
        {
            PlaceDoor(conn.doorA);
            PlaceDoor(conn.doorB);
        }
    }
 
    private void PlaceDoor(DoorCandidate door)
    {
        if (door == null) return;
        
        Vector3 worldPos = new Vector3(door.wallPos.x, door.wallPos.y, 0f);
        GameObject d = _objectPool.Spawn(_doorPrefab, _doorRoot, worldPos, Quaternion.identity);
 
        float angle = door.dir switch
        {
            DoorDir.Up    => 0f,
            DoorDir.Down  => 180f,
            DoorDir.Left  => 90f,
            _             => -90f
        };
        d.transform.rotation = Quaternion.Euler(0f, 0f, angle);
 
        if (door.owner != null)
        {
            if (!_roomDoors.ContainsKey(door.owner))
                _roomDoors[door.owner] = new List<GameObject>();
            _roomDoors[door.owner].Add(d);
        }
    }
 
    public void CloseDoors(RoomNode room)
    {
        if (!_roomDoors.TryGetValue(room, out var doors)) return;
        foreach (var d in doors)
            if (d != null) d.SetActive(false);
    }
 
    public void OpenDoors(RoomNode room)
    {
        if (!_roomDoors.TryGetValue(room, out var doors)) return;
        foreach (var d in doors)
            if (d != null) d.SetActive(true);
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