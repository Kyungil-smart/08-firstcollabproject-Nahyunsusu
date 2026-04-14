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
            PlayBossBGM();
        }
    }

    private void PlayBossBGM()
    {
        // 현재 씬 이름 기준으로 스테이지 인덱스 파악
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        int stageIndex = sceneName switch
        {
            "Stage1" => 1,
            "Stage2" => 2,
            "Stage3" => 3,
            _        => 0
        };

        if (stageIndex > 0)
            BGMManager.Instance?.PlayBossBGM(stageIndex);
    }

    private IEnumerator CloseDoorsDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        doorController.CloseDoors(room);
    }
}