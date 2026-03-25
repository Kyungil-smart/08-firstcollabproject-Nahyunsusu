using System.Collections.Generic;
using UnityEngine;

// 임시 맵용 타일 생성기
// 방 / 복도 바닥과 벽을 1x1 단위로 배치
public class TileMapGenerator : MonoBehaviour
{
    [Header("타일 프리팹")]
    public GameObject floorTilePrefab;
    public GameObject wallTilePrefab;

    [Header("벽 설정")]
    public int wallThickness = 2;
    
    private List<GameObject> _tiles = new List<GameObject>();

    public void Generate(MapGraph graph, List<CorridorData> corridors)
    {
        Clear();

        foreach (RoomNode room in graph.allRooms)
        {
            if (room.size.x == 0 || room.size.y == 0)
                continue;

            GenerateRoomFloor(room);
            GenerateRoomWall(room);
        }

        foreach (CorridorData corridor in corridors)
        {
            GenerateCorridorFloor(corridor);
            GenerateCorridorWall(corridor);
        }
    }

    // 방 바닥 채우기
    private void GenerateRoomFloor(RoomNode room)
    {
        int startX = Mathf.RoundToInt(room.worldPosition.x - room.size.x * 0.5f);
        int startY = Mathf.RoundToInt(room.worldPosition.y - room.size.y * 0.5f);

        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
            {
                PlaceTile(floorTilePrefab, new Vector2(startX + x + 0.5f, startY + y + 0.5f));
            }
        }
    }

    // 방 외곽 벽 생성
    private void GenerateRoomWall(RoomNode room)
    {
        int startX = Mathf.RoundToInt(room.worldPosition.x - room.size.x * 0.5f);
        int startY = Mathf.RoundToInt(room.worldPosition.y - room.size.y * 0.5f);
        int endX = startX + room.size.x;
        int endY = startY + room.size.y;

        for (int w = 1; w <= wallThickness; w++)
        {
            // 위 / 아래
            for (int x = startX - w; x < endX + w; x++)
            {
                PlaceTile(wallTilePrefab, new Vector2(x + 0.5f, endY + w - 1 + 0.5f));
                PlaceTile(wallTilePrefab, new Vector2(x + 0.5f, startY - w + 0.5f));
            }
            
            // 왼쪽 / 오른쪽
            for (int y = startY - w + 1; y < endY + w - 1; y++)
            {
                PlaceTile(wallTilePrefab, new Vector2(startX - w + 0.5f, y + 0.5f));
                PlaceTile(wallTilePrefab, new Vector2(endX + w - 1 + 0.5f, y + 0.5f));
            }
        }
    }
    
    // 복도 바닥 채우기
    private void GenerateCorridorFloor(CorridorData corridor)
    {
        int halfWidth = Mathf.RoundToInt(corridor.width * 0.5f);

        for (int i = 0; i < corridor.points.Length - 1; i++)
        {
            Vector2 from = corridor.points[i];
            Vector2 to = corridor.points[i + 1];
            
            FillSegment(floorTilePrefab, from, to, halfWidth);
        }
    }
    
    // 복도 바깥쪽 벽 생성
    private void GenerateCorridorWall(CorridorData corridor)
    {
        int halfWidth = Mathf.RoundToInt(corridor.width * 0.5f);
        int wallOuterEnd = halfWidth + wallThickness;

        for (int i = 0; i < corridor.points.Length - 1; i++)
        {
            Vector2 from = corridor.points[i];
            Vector2 to = corridor.points[i + 1];

            for (int w = halfWidth; w < wallOuterEnd; w++)
            {
                FillSegment(wallTilePrefab, from, to, w);
            }
        }
    }
    
    // 직각 구간 기준으로 타일 배치
    // 현재 복도 경로가 수평 / 수직이라는 전제로 사용
    private void FillSegment(GameObject prefab, Vector2 from, Vector2 to, int halfWidth)
    {
        bool isHorizontal = Mathf.Abs(to.y - from.y) < 0.1f;
        
        int startX = Mathf.RoundToInt(Mathf.Min(from.x, to.x));
        int startY = Mathf.RoundToInt(Mathf.Min(from.y, to.y));
        int endX = Mathf.RoundToInt(Mathf.Max(from.x, to.x));
        int endY = Mathf.RoundToInt(Mathf.Max(from.y, to.y));

        if (isHorizontal)
        {
            for (int x = startX; x < endX; x++)
            {
                for (int w = -halfWidth; w < halfWidth; w++)
                {
                    PlaceTile(prefab, new Vector2(x + 0.5f, startY + w + 0.5f));
                }
            }
        }
        else
        {
            for (int y = startY; y < endY; y++)
            {
                for (int w = -halfWidth; w < halfWidth; w++)
                {
                    PlaceTile(prefab, new Vector2(startX + w + 0.5f, y + 0.5f));
                }
            }
        }
    }

    private void PlaceTile(GameObject prefab, Vector2 position)
    {
        if (prefab == null)
            return;
        
        GameObject tile = Instantiate(prefab, new Vector3(position.x, position.y, 0f), Quaternion.identity);
        tile.transform.SetParent(transform);
        _tiles.Add(tile);
    }
    
    public void Clear()
    {
        foreach (GameObject tile in _tiles)
        {
            if (tile != null)
                Destroy(tile);
        }
        
        _tiles.Clear();
    }
}
