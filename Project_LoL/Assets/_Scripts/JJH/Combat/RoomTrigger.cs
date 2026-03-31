using System.Collections;
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
        if (data.state != RoomState.Unvisited) return;

        StartCoroutine(CloseDoorsDelayed());
        RoomClearManager.Instance?.StartRoom(room);
    }

    private IEnumerator CloseDoorsDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        doorController.CloseDoors(room);
    }
}