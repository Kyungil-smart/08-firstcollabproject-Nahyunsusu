using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapUI : MonoBehaviour
{
    [SerializeField] private RectTransform _mapContainer;
    [SerializeField] private GameObject _roomIconPrefab;
    [SerializeField] private float _positionScale = 0.5f;
    [SerializeField] private float _iconSize = 8f;
    
    [SerializeField] private float _panelBottomMargin = 20f;

    [SerializeField] private Color _colorUnvisited = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color _colorInProgress = new Color(1f, 0.9f, 0f, 1f);
    [SerializeField] private Color _colorCleared = new Color(0.3f, 0.6f, 1f, 1f);
    [SerializeField] private Color _colorCurrent = new Color(1f, 1f, 1f, 1f);

    private MapManager _mapManager;
    private Dictionary<RoomNode, Image> _roomIcons = new Dictionary<RoomNode, Image>();
    private RoomNode _currentRoom;

    private void Awake()
    {
        _mapManager = FindAnyObjectByType<MapManager>();
    }

    public void BuildMiniMap(List<RoomNode> rooms)
    {
        if (_mapContainer == null || _roomIconPrefab == null) return;

        foreach (Transform child in _mapContainer)
            Destroy(child.gameObject);
        _roomIcons.Clear();

        RoomNode startRoom = null;
        foreach (var room in rooms)
        {
            if (room.roomData.roomType == RoomType.Start)
            {
                startRoom = room;
                break;
            }
        }

        Vector2 startCenter = startRoom != null
            ? new Vector2(startRoom.gridOrigin.x + startRoom.size.x * 0.5f, startRoom.gridOrigin.y + startRoom.size.y * 0.5f)
            : Vector2.zero;

        foreach (var room in rooms)
        {
            GameObject icon = Instantiate(_roomIconPrefab, _mapContainer, false);
            if (icon == null) continue;

            RectTransform rt = icon.GetComponent<RectTransform>();
            if (rt == null) continue;

            Vector2 center = new Vector2(
                room.gridOrigin.x + room.size.x * 0.5f,
                room.gridOrigin.y + room.size.y * 0.5f
            );

            Vector2 relative = (center - startCenter) * _positionScale;
            rt.anchoredPosition = new Vector2(
                relative.x,
                _panelBottomMargin + relative.y
            );
            rt.sizeDelta = new Vector2(_iconSize, _iconSize);

            Image img = icon.GetComponent<Image>();
            img.color = _colorUnvisited;
            _roomIcons[room] = img;
        }
    }

    public void UpdateMiniMap(RoomNode currentRoom)
    {
        _currentRoom = currentRoom;

        foreach (var pair in _roomIcons)
        {
            RoomNode room = pair.Key;
            Image img = pair.Value;
            RoomRuntimeData data = _mapManager.GetRuntimeData(room);

            if (room == currentRoom)
            {
                img.color = _colorCurrent;
                continue;
            }

            if (data == null)
            {
                img.color = _colorUnvisited;
                continue;
            }

            img.color = data.state switch
            {
                RoomState.InProgress => _colorInProgress,
                RoomState.Cleared => _colorCleared,
                _ => _colorUnvisited
            };
        }
    }

    private Vector2 GetOffset(List<RoomNode> rooms)
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;

        foreach (var room in rooms)
        {
            if (room.roomData.roomType == RoomType.Start) continue;
            minX = Mathf.Min(minX, room.gridOrigin.x);
            minY = Mathf.Min(minY, room.gridOrigin.y);
        }

        return new Vector2(-minX, -minY);
    }
}