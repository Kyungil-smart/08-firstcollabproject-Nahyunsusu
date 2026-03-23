using UnityEngine;

// 방 하나에 대한 설정 데이터
// 그래프 기반 맵 생성 시 노드 단위로 사용
[CreateAssetMenu(fileName = "NewRoomData", menuName = "Map/RoomData")]
public class RoomData : ScriptableObject
{
    public string roomName;   // 표시용 이름 (디버그/로그용)
    
    public RoomType roomType; // 방 타입 (이걸 기준으로 내용 분기)
}