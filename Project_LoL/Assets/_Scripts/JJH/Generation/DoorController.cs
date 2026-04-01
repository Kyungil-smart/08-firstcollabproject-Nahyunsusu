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

        var placed = new HashSet<(RoomNode, Vector2Int)>();

        foreach (var conn in connections)
        {
            RegisterDoor(conn.doorA, conn.corridorWidth, placed);
            RegisterDoor(conn.doorB, conn.corridorWidth, placed);
        }
    }
    
    private void RegisterDoor(DoorCandidate door, int width, HashSet<(RoomNode, Vector2Int)> placed)
    {
        if (door == null) return;

        if (door.owner != null)
        {
            if (!_roomDoors.ContainsKey(door.owner))
                _roomDoors[door.owner] = new List<DoorObject>();
        }

        var key = (door.owner, door.wallPos);
        if (!placed.Add(key)) return;

        PlaceDoor(door, width);
    }
 
    private void PlaceDoor(DoorCandidate door, int width)
    {
        if (door == null) return;

        Vector3 worldPos = CalcPos(door, width);
        Debug.Log($"dir:{door.dir} wallPos:{door.wallPos} width:{width} owner:{door.owner?.nodeId}");
        GameObject d = _objectPool.Spawn(_doorPrefab, _doorRoot, worldPos, Quaternion.identity);
        d.SetActive(false);

        float angle = door.dir switch
        {
            DoorDir.Up    => 0f,
            DoorDir.Down  => 180f,
            DoorDir.Left  => 90f,
            _             => -90f
        };

        DoorObject doorObj = d.GetComponent<DoorObject>();
        if (doorObj != null)
            doorObj.Setup(width, door.dir);

        d.SetActive(true);

        if (door.owner != null)
        {
            if (!_roomDoors.ContainsKey(door.owner))
                _roomDoors[door.owner] = new List<DoorObject>();
            if (doorObj != null)
                _roomDoors[door.owner].Add(doorObj);
        }
    }
    
    private Vector3 CalcPos(DoorCandidate door, int width)
    {
        return new Vector3(door.wallPos.x, door.wallPos.y, 0f);
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