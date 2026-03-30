using UnityEngine;

public class BossWarningState : BossStateBase
{
    private float _timer;

    public BossWarningState(BossFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _timer = 0f;
        _fsm.SelectRandomSkill();

        if (_fsm.selectedSkill == null)
        {
            _fsm.ChangeState(BossStateType.Chase);
            return;
        }

        if (_fsm.playerTransform != null)
        {
            _fsm.lockedFacingDir = ((Vector2)_fsm.playerTransform.position - (Vector2)_fsm.transform.position).normalized;
            _fsm.FlipToPlayer();
        }

        if (_fsm.warningSignObj != null)
        {
            _fsm.warningSignObj.SetActive(true);
            
            if (_fsm.selectedSkill.skillType == MonsterSkillType.Melee)
            {
                Vector2 offset = _fsm.lockedFacingDir * (_fsm.selectedSkill.range * 0.5f);
                _fsm.warningSignObj.transform.localPosition = new Vector3(offset.x, offset.y, 0);
            }
            else
            {
                _fsm.warningSignObj.transform.localPosition = Vector3.zero;
            }

            _fsm.warningSignObj.transform.localScale = Vector3.zero;
        }
    }

    public override void Update()
    {
        if (_fsm.selectedSkill == null) return;

        _timer += Time.deltaTime;
        float progress = Mathf.Clamp01(_timer / _fsm.selectedSkill.warningDuration);

        if (_fsm.warningSignObj != null)
        {
            Vector3 targetScale = new Vector3(_fsm.selectedSkill.damageRangeX, _fsm.selectedSkill.damageRangeY, 1f);
            _fsm.warningSignObj.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress);
        }

        if (_timer >= _fsm.selectedSkill.warningDuration)
            _fsm.ChangeState(BossStateType.Attack);
    }

    public override void Exit() { }
}