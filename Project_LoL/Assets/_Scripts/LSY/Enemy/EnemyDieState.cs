using UnityEngine;

public class EnemyDieState : EnemyStateBase
{
    public EnemyDieState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _fsm.rigid.linearVelocity = Vector2.zero;

        // 경험치 매니저 연결 필요

        // 골드 매니저 연결 필요

        // 던전 퇴장 시 결과 UI에 표시될 처치 수 기록

        if (EnemyPool.Instance != null)
            EnemyPool.Instance.Return(_fsm.gameObject);
        else
            Object.Destroy(_fsm.gameObject);
    }
}