using System.Collections.Generic;
using UnityEngine;

// 방에 생성된 문 위치를 시각화
// 추후 실제 문 에셋으로 교체 예정
public class DoorVisualizer : MonoBehaviour
{
    [Header("문 크기")]
    public float doorSize = 2f;

    private List<GameObject> _doorObjects = new List<GameObject>();
    private Sprite _cachedSprite;

    public void Visualize(MapGraph graph)
    {
        Clear();

        foreach (RoomNode room in graph.allRooms)
        {
            foreach (DoorData door in room.doors)
            {
                Vector2 worldPos = room.worldPosition + door.localPosition;

                GameObject obj = new GameObject($"Door_{room.nodeId}_to_{door.connectedRoom.nodeId}");
                obj.transform.SetParent(transform);
                obj.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);
                obj.transform.localScale = new Vector3(doorSize, doorSize, 1f);

                SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
                sr.sprite = GetSprite();
                sr.color = GetDoorColor(door.direction);
                sr.sortingOrder = 20;

                _doorObjects.Add(obj);
            }
        }
    }

    // 문 상태 변경 (외부 로직에서 제어)
    public void SetDoorState(RoomNode room, bool isOpen)
    {
        Debug.Log($"[DoorVisualizer] {room.nodeId} 문 {(isOpen ? "열림" : "닫힘")}");
        // 추후 실제 문 오브젝트 열림/닫힘 처리
    }

    // 방향별 색상 구분 (디버그용)
    private Color GetDoorColor(DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.Up:    return new Color(1f, 1f, 0f);
            case DoorDirection.Down:  return new Color(1f, 0.5f, 0f);
            case DoorDirection.Left:  return new Color(0f, 1f, 1f);
            case DoorDirection.Right: return new Color(1f, 0f, 1f);
            default:                  return Color.white;
        }
    }

    private Sprite GetSprite()
    {
        if (_cachedSprite != null)
            return _cachedSprite;

        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        _cachedSprite = Sprite.Create(
            tex,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );

        return _cachedSprite;
    }

    public void Clear()
    {
        foreach (GameObject obj in _doorObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        _doorObjects.Clear();
    }
}