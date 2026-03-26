using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState : EnemyStateBase
{
    private List<Vector2> _path = new List<Vector2>();
    private int _pathIndex;

    private const float PATH_UPDATE_INTERVAL = 0.3f;
    private float _pathUpdateTimer;

    public EnemyChaseState(EnemyFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _fsm.animator?.SetBool("1_Move", true);

        // 몬스터마다 타이머 초기값 랜덤 분산 → 동시 경로 계산 방지
        _pathUpdateTimer = Random.Range(0f, PATH_UPDATE_INTERVAL);
        RequestPath();
    }

    public override void Update()
    {
        if (_fsm.isPlayerInAttackRange)
        {
            _fsm.ChangeState(EnemyStateType.Attack);
            return;
        }
        if (!_fsm.isPlayerInDetectRange)
        {
            _fsm.ChangeState(EnemyStateType.Idle);
            return;
        }

        _pathUpdateTimer += Time.deltaTime;
        if (_pathUpdateTimer >= PATH_UPDATE_INTERVAL)
        {
            _pathUpdateTimer = 0f;
            RequestPath();
        }

        MoveAlongPath();
        _fsm.FlipToPlayer();
    }

    public override void Exit()
    {
        _fsm.rigid.linearVelocity = Vector2.zero;
        _fsm.animator?.SetBool("1_Move", false);
        _path.Clear();
    }

    private void RequestPath()
    {
        if (_fsm.pathfinder == null || _fsm.playerTransform == null) return;

        List<Vector2> newPath = _fsm.pathfinder.FindPath(
            _fsm.transform.position,
            _fsm.playerTransform.position
        );

        if (newPath == null || newPath.Count == 0)
        {
            _path.Clear();
            return;
        }

        _path = newPath;

        // 현재 위치와 가장 가까운 웨이포인트부터 시작
        // → 경로 재계산 시 갑자기 시작점으로 돌아가는 문제 방지
        _pathIndex = GetClosestPathIndex();
    }

    // 현재 위치에서 가장 가까운 웨이포인트 인덱스 반환
    private int GetClosestPathIndex()
    {
        int closest = 0;
        float minDist = float.MaxValue;

        for (int i = 0; i < _path.Count; i++)
        {
            float dist = Vector2.Distance(_fsm.transform.position, _path[i]);
            if (dist < minDist)
            {
                minDist = dist;
                closest = i;
            }
        }

        return closest;
    }

    private void MoveAlongPath()
    {
        // 경로 없으면 직선 이동으로 폴백
        if (_path == null || _path.Count == 0 || _pathIndex >= _path.Count)
        {
            if (_fsm.playerTransform != null)
            {
                Vector2 dir = ((Vector2)_fsm.playerTransform.position
                               - (Vector2)_fsm.transform.position).normalized;
                _fsm.rigid.linearVelocity = dir * _fsm.data.moveSpeed;
            }
            return;
        }

        Vector2 target  = _path[_pathIndex];
        Vector2 current = _fsm.transform.position;
        Vector2 moveDir = (target - current).normalized;

        _fsm.rigid.linearVelocity = moveDir * _fsm.data.moveSpeed;

        if (Vector2.Distance(current, target) < 0.3f)
            _pathIndex++;
    }
}