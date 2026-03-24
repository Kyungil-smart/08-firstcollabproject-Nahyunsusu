using System.Collections.Generic;
using UnityEngine;

// 그래프의 방 위치와 크기를 흰색 사각형으로 시각화
// 에셋 확정 전 임시용, 추후 타일맵으로 교체 예정
public class RoomVisualizer : MonoBehaviour
{
    private List<GameObject> _roomObjects = new List<GameObject>();
    private Dictionary<Vector2Int, Sprite> _spriteCache = new Dictionary<Vector2Int, Sprite>();

    // 현재 그래프 기준으로 방 시각화
    public void Visualize(MapGraph graph)
    {
        Clear();

        foreach (RoomNode room in graph.allRooms)
        {
            if (room.size.x == 0 || room.size.y == 0)
                continue;

            GameObject obj = new GameObject($"Visual_{room.nodeId}");
            obj.transform.SetParent(transform);
            obj.transform.position = new Vector3(room.worldPosition.x, room.worldPosition.y, 0f);
            obj.transform.localScale = new Vector3(room.size.x, room.size.y, 1f);

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateRectSprite(room.size.x, room.size.y);
            sr.color = GetRoomColor(room.roomData.roomType);
            sr.sortingOrder = GetSortingOrder(room.roomData.roomType);

            _roomObjects.Add(obj);
        }
    }
    
    private int GetSortingOrder(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start:   return 10;
            case RoomType.Repair:  return 10;
            case RoomType.Boss:    return 10;
            case RoomType.Combat:  return 5;
            default:               return 0;
        }
    }

    // 방 타입에 따라 표시 색상 구분
    private Color GetRoomColor(RoomType type)
    {
        switch (type)
        {
            case RoomType.Start:   return new Color(0.3f, 0.8f, 0.5f); // 초록
            case RoomType.Combat:  return new Color(1f,   1f,   1f);   // 흰색
            case RoomType.Repair:  return new Color(0.5f, 0.6f, 1f);   // 파랑
            case RoomType.Boss:    return new Color(1f,   0.3f, 0.3f); // 빨강
            default:               return Color.white;
        }
    }

    // 같은 크기 방은 같은 스프라이트를 재사용
    private Sprite CreateRectSprite(int width, int height)
    {
        Vector2Int key = new Vector2Int(width, height);
        
        if (_spriteCache.TryGetValue(key, out Sprite cachedSprite))
            return cachedSprite;

        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );
        
        _spriteCache.Add(key, sprite);
        return sprite;
    }
    
    // 기존 시각화 오브젝트 제거
    public void Clear()
    {
        foreach (GameObject obj in _roomObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        _roomObjects.Clear();
    }
}