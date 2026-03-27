using UnityEngine;

[CreateAssetMenu(fileName = "NewRoomData", menuName = "Map/RoomData")]
public class RoomData : ScriptableObject
{
    public string roomName;
    public RoomType roomType;
    
    [Header("방 크기 설정")]
    public Vector2Int fixedSize;
    public Vector2Int minSize;
    public Vector2Int maxSize;
    
    [Header("시각적 프리팹 (필요 시)")]
    public GameObject floorPrefab;

    [Header("고정 문 설정")]
    public bool useFixedDoor;
    public int fixedDoorDirection;
    public Vector2Int fixedDoorLocalPosition;

    public Vector2Int GetRandomSize()
    {
        if (roomType == RoomType.Combat || roomType == RoomType.Repair)
        {
            return new Vector2Int(
                Random.Range(minSize.x, maxSize.x + 1),
                Random.Range(minSize.y, maxSize.y + 1)
            );
        }

        return fixedSize;
    }
}