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

        _pathIndex = GetClosestPathIndex();
    }

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
