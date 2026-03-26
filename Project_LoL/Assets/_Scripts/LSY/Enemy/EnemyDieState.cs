using System.Collections;
using UnityEngine;

// 죽음 상태
// 애니메이션 재생 후 풀에 반환
public class EnemyDieState : EnemyStateBase
{
    private const float DEFAULT_DEATH_DURATION = 0.5f;

    public EnemyDieState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _fsm.rigid.linearVelocity = Vector2.zero;
        _fsm.animator?.SetTrigger("4_Death");
        _fsm.animator?.SetBool("isDeath", true);

        // 경험치 매니저 연결 필요
        // ExperienceManager.Instance.AddExp(_fsm.data.expReward);

        // 골드 매니저 연결 필요
        // GoldManager.Instance.AddGold(_fsm.data.goldReward);

        // 던전 퇴장 시 결과 UI에 표시될 처치 수 기록
        // DungeonManager.Instance.AddKillCount();

        // 방 클리어 카운트 감소
        if (RoomClearManager.Instance != null)
            RoomClearManager.Instance.OnEnemyDied(_fsm.currentRoom);

        _fsm.StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        // SetTrigger 직후엔 클립이 전환 전이라 못 찾을 수 있음
        // → 한 프레임 대기 후 클립 길이 조회
        yield return null;

        float deathAnimDuration = GetDeathAnimLength();
        yield return new WaitForSeconds(deathAnimDuration);

        if (EnemyPool.Instance != null)
            EnemyPool.Instance.Return(_fsm.gameObject);
        else
            Object.Destroy(_fsm.gameObject);
    }

    private float GetDeathAnimLength()
    {
        if (_fsm.animator == null) return DEFAULT_DEATH_DURATION;

        // RuntimeAnimatorController에서 클립 직접 탐색
        // GetCurrentAnimatorClipInfo보다 안정적
        RuntimeAnimatorController rac = _fsm.animator.runtimeAnimatorController;
        if (rac == null) return DEFAULT_DEATH_DURATION;

        foreach (AnimationClip clip in rac.animationClips)
        {
            string name = clip.name.ToUpper();
            if (name.Contains("DEATH") || name.Contains("DIE"))
                return clip.length;
        }

        return DEFAULT_DEATH_DURATION;
    }
}