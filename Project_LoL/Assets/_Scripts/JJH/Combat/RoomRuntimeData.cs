public enum RoomState
{
    Unvisited,   // 미입장
    InProgress,  // 진행 중
    Cleared      // 클리어
}

public class RoomRuntimeData
{
    public RoomNode roomNode;       // 어떤 방의 런타임 정보인지 참조
    public RoomState state;         // 현재 진행 상태

    public int aliveMonsterCount;   // 현재 살아있는 몬스터 수
    public bool isCombatActive;     // 전투 중 여부 (문 닫힘 기준)

    public RoomRuntimeData(RoomNode node)
    {
        roomNode = node;
        state = RoomState.Unvisited;
        aliveMonsterCount = 0;
        isCombatActive = false;
    }

    // 전투 시작 시 호출
    public void StartCombat(int monsterCount)
    {
        state = RoomState.InProgress;
        aliveMonsterCount = monsterCount;
        isCombatActive = monsterCount > 0;
    }

    // 몬스터가 죽을 때마다 호출
    public void OnMonsterDead()
    {
        if (aliveMonsterCount <= 0)
            return;

        aliveMonsterCount--;

        if (aliveMonsterCount == 0)
        {
            isCombatActive = false;
            state = RoomState.Cleared;
        }
    }
}