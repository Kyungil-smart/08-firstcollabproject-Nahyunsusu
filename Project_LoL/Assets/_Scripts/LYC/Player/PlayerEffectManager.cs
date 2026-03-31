using System;
using DG.Tweening;
using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{
	[SerializeField] private ParticleSystem _dashParticle;

	private PlayerFSM _fsm;
	private SpriteRenderer _playerRenderer;
	private Animator _animator;

	private float _actionEffectDuration = 0.2f;
	private readonly int _fFacingX = Animator.StringToHash("FacingX");
	private readonly int _fFacingY = Animator.StringToHash("FacingY");
	private readonly int _fMoveX = Animator.StringToHash("MoveX");
	private readonly int _fMoveY = Animator.StringToHash("MoveY");
	private readonly int _bIsMoving = Animator.StringToHash("IsMoving");
	private readonly int _tAttack = Animator.StringToHash("Attack");

	private void Awake()
	{
		_playerRenderer = GetComponentInChildren<SpriteRenderer>();
		_fsm = GetComponent<PlayerFSM>();
		_animator = GetComponent<Animator>();
	}

	private void Update()
	{
		_animator.SetFloat(_fFacingX, _fsm.FacingDir.x);
		_animator.SetFloat(_fFacingY, _fsm.FacingDir.y);
		_animator.SetFloat(_fMoveX, _fsm.MoveInput.x);
		_animator.SetFloat(_fMoveY, _fsm.MoveInput.y);
		_animator.SetBool(_bIsMoving, _fsm.MoveInput.sqrMagnitude > 0);
	}

	public void PlayHitEffect()
	{
		_playerRenderer.color = new Color(0.5f, 0, 0, 1);
		_playerRenderer.DOColor(Color.white, _actionEffectDuration).SetEase(Ease.InCubic);
	}

	public void PlayDashEffect()
	{
		var particlePosition = _dashParticle.transform.localPosition;
		particlePosition.x = _fsm.FacingDir.x switch
		{
			> 0 => -0.5f,
			< 0 => 0.5f,
			_ => 0
		};
		_dashParticle.Play();

		_playerRenderer.transform.localScale = Vector3.one * 0.93f;
		_playerRenderer.transform.DOScale(Vector3.one, _actionEffectDuration).SetEase(Ease.InCubic);
	}
}