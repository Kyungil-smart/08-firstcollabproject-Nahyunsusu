using UnityEngine;

// 방 단위 플레이어 진입 감지
public class RoomTrigger : MonoBehaviour
{
    public RoomNode room;
    public MapManager mapManager;
    public DoorController doorController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (room == null || mapManager == null || doorController == null) return;

        if (room.roomData.roomType != RoomType.Combat) return;

        RoomRuntimeData data = mapManager.GetRuntimeData(room);
        if (data == null) return;

        // 이미 진행 중이거나 클리어된 방은 무시
        if (data.state != RoomState.Unvisited) return;

        // 방 진입 시 바로 문 닫기 (몬스터 수와 무관)
        doorController.CloseDoors(room);
        RoomClearManager.Instance?.StartRoom(room);
    }
}