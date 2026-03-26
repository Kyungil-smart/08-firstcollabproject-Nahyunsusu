using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("문 프리팹")]
    public GameObject doorPrefab;

    [Header("문 색상")]
    public Color closedColor = Color.red;
    public Color openColor   = new Color(0f, 0f, 0f, 0f);

    private Dictionary<RoomNode, List<GameObject>> _doorObjects
        = new Dictionary<RoomNode, List<GameObject>>();

    public void BuildDoors(MapGraph graph)
    {
        _doorObjects.Clear();

        HashSet<string> processed = new HashSet<string>();

        foreach (RoomNode room in graph.allRooms)
        {
            foreach (DoorData door in room.doors)
            {
                string key = GetDoorKey(room, door.connectedRoom);
                if (processed.Contains(key))
                    continue;

                processed.Add(key);

                Vector2 worldPos = room.worldPosition + door.localPosition;
                GameObject obj   = CreateDoorObject(worldPos, door.direction);

                AddDoorToRoom(room, obj);
                AddDoorToRoom(door.connectedRoom, obj);
            }
        }
    }

    public void CloseDoors(RoomNode room)
    {
        if (!_doorObjects.TryGetValue(room, out List<GameObject> doors))
            return;

        foreach (GameObject door in doors)
        {
            if (door == null) continue;
            SetDoorState(door, true);
        }
    }

    public void OpenDoors(RoomNode room)
    {
        if (!_doorObjects.TryGetValue(room, out List<GameObject> doors))
            return;

        foreach (GameObject door in doors)
        {
            if (door == null) continue;
            SetDoorState(door, false);
        }
    }

    private GameObject CreateDoorObject(Vector2 worldPos, DoorDirection direction)
    {
        GameObject obj = Instantiate(doorPrefab, new Vector3(worldPos.x, worldPos.y, 0f), Quaternion.identity);
        obj.transform.SetParent(transform);

        if (direction == DoorDirection.Up || direction == DoorDirection.Down)
            obj.transform.localScale = new Vector3(5f, 1f, 1f);
        else
            obj.transform.localScale = new Vector3(1f, 5f, 1f);

        SetDoorState(obj, false);

        return obj;
    }

    private void SetDoorState(GameObject door, bool closed)
    {
        SpriteRenderer sr = door.GetComponent<SpriteRenderer>();
        Collider2D col    = door.GetComponent<Collider2D>();

        if (sr != null) sr.color    = closed ? closedColor : openColor;
        if (col != null) col.enabled = closed;
    }

    private void AddDoorToRoom(RoomNode room, GameObject obj)
    {
        if (!_doorObjects.ContainsKey(room))
            _doorObjects[room] = new List<GameObject>();

        _doorObjects[room].Add(obj);
    }

    private string GetDoorKey(RoomNode a, RoomNode b)
    {
        return string.Compare(a.nodeId, b.nodeId) < 0
            ? $"{a.nodeId}_{b.nodeId}"
            : $"{b.nodeId}_{a.nodeId}";
    }
}