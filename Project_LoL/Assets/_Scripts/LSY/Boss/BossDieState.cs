using System.Collections;
using UnityEngine;

public class BossDieState : BossStateBase
{
    private const float DEFAULT_DEATH_DURATION = 2.0f;
    
    public static event System.Action OnBossDied;

    public BossDieState(BossFSM fsm) : base(fsm) { }

    public override void Enter()
    {
        _fsm.rigid.linearVelocity = Vector2.zero;
        _fsm.rigid.bodyType = RigidbodyType2D.Kinematic;
        _fsm.animator?.SetTrigger("4_Death");
        _fsm.animator?.SetBool("isDeath", true);

        if (RoomClearManager.Instance != null)
            RoomClearManager.Instance.OnEnemyDied(null, _fsm.data.goldReward, _fsm.data.expReward);

        OnBossDied?.Invoke();

        _fsm.StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        yield return null;

        float deathAnimDuration = GetDeathAnimLength();
        
        yield return new WaitForSeconds(deathAnimDuration);

        Object.Destroy(_fsm.gameObject, 0.5f);
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
            {
                return clip.length;
            }
        }

        return DEFAULT_DEATH_DURATION;
    }

    public override void Update() { }

    public override void Exit() { }
}