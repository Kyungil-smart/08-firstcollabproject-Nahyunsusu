using UnityEngine;

// 방 하나에 대한 설정 데이터
// 그래프 기반 맵 생성 시 노드 단위로 사용
[CreateAssetMenu(fileName = "NewRoomData", menuName = "Map/RoomData")]
public class RoomData : ScriptableObject
{
    public string roomName;   // 표시용 이름 (디버그/로그용)
    
    public RoomType roomType; // 방 타입 (이걸 기준으로 내용 분기)
    
    // stageIndex:
    // 절차적 생성 시 난이도 구간 구분용
    // (초반 / 중반 / 후반 분기 기준)
    public int stageIndex;    // 몇 번째 스테이지에서 등장하는지
}