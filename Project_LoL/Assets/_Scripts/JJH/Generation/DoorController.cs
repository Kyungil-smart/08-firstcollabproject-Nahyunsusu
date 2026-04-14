using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private MapObjectPool _objectPool;
    [SerializeField] private GameObject _doorPrefab;
    [SerializeField] private Transform _doorRoot;

    private Dictionary<RoomNode, List<DoorObject>> _roomDoors = new Dictionary<RoomNode, List<DoorObject>>();

    public void BuildDoors(List<DoorPlacement> placements)
    {
        _objectPool.ReleaseChildren(_doorRoot);
        _roomDoors.Clear();

        foreach (var placement in placements)
            PlaceDoor(placement);
    }

    private void PlaceDoor(DoorPlacement placement)
    {
        Vector3 worldPos = new Vector3(placement.pivot.x, placement.pivot.y, 0f);

        GameObject d = _objectPool.Spawn(_doorPrefab, _doorRoot, worldPos, Quaternion.identity);

        DoorObject doorObj = d.GetComponent<DoorObject>();
        if (doorObj != null)
            doorObj.Setup(placement.width, placement.dir);

        if (placement.owner != null)
        {
            if (!_roomDoors.ContainsKey(placement.owner))
                _roomDoors[placement.owner] = new List<DoorObject>();
            if (doorObj != null)
                _roomDoors[placement.owner].Add(doorObj);
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