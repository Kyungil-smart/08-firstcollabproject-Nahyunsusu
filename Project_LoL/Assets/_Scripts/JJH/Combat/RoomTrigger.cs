using System.Collections;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public RoomNode room;
    public MapManager mapManager;
    public DoorController doorController;
    public MiniMapUI miniMapUI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (room == null || mapManager == null || doorController == null) return;

        RoomRuntimeData data = mapManager.GetRuntimeData(room);
        if (data == null) return;

        miniMapUI?.UpdateMiniMap(room);

        if (data.state != RoomState.Unvisited) return;

        if (room.roomData.roomType == RoomType.Combat)
        {
            StartCoroutine(CloseDoorsDelayed());
            RoomClearManager.Instance?.StartRoom(room);
        }
        else if (room.roomData.roomType == RoomType.Boss)
        {
            StartCoroutine(CloseDoorsDelayed());
        }
    }

    private IEnumerator CloseDoorsDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        doorController.CloseDoors(room);
    }
}