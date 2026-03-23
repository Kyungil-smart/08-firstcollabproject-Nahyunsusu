// 방의 역할 구분용
// 맵 생성 시 어떤 이벤트를 넣을지 기준이 됨
public enum RoomType
{
    Start,      // 시작 지점
    Combat,     // 일반 전투
    Shop,       // 상점
    Upgrade,    // 강화
    Boss        // 보스 방 (마지막)
}