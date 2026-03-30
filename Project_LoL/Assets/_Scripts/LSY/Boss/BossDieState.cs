using System.Collections;
using UnityEngine;

public class BossDieState : BossStateBase
{
    private const float DEFAULT_DEATH_DURATION = 0.5f;

    public BossDieState(BossFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _fsm.rigid.linearVelocity = Vector2.zero;
        _fsm.animator?.SetTrigger("4_Death");
        _fsm.animator?.SetBool("isDeath", true);

        // TODO: 경험치 매니저 연결 필요
        // TODO: 골드 매니저 연결 필요
        // TODO: MapManager.OnBossDefeated() 연결 필요

        _fsm.StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return null;

        float duration = GetDeathAnimLength();
        yield return new UnityEngine.WaitForSeconds(duration);

        Object.Destroy(_fsm.gameObject);
    }

    private float GetDeathAnimLength()
    {
        if (_fsm.animator == null) return DEFAULT_DEATH_DURATION;

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
