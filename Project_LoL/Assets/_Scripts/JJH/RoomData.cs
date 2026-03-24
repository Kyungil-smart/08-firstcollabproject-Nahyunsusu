using UnityEngine;

// 방 하나에 대한 설정 데이터
// 그래프 기반 맵 생성 시 노드 단위로 사용
[CreateAssetMenu(fileName = "NewRoomData", menuName = "Map/RoomData")]
public class RoomData : ScriptableObject
{
    public string roomName;   // 표시용 이름 (디버그/로그용)
    public RoomType roomType; // 방 타입 (이걸 기준으로 내용 분기)
    
    [Header("방 크기")]
    public Vector2Int fixedSize;    // 전투 방을 제외한 고정 크기 방
    public Vector2Int minSize;      // 전투 방 랜덤 크기 범위 (최소)
    public Vector2Int maxSize;      // 전투 방 랜덤 크기 범위 (최대)

    // 방 타입에 따라 크기 반환
    // 전투 방만 가변 크기, 나머지는 고정 크기 사용
    public Vector2Int GetRoomSize()
    {
        if (roomType == RoomType.Combat || roomType == RoomType.Repair)
        {
            int width  = Random.Range(minSize.x, maxSize.x + 1);
            int height = Random.Range(minSize.y, maxSize.y + 1);
            return new Vector2Int(width, height);
        }

        return fixedSize;
    }
}