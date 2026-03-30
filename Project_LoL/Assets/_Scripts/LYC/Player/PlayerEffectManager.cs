using DG.Tweening;
using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{
	[SerializeField] private ParticleSystem _dashParticle;

	private PlayerFSM _fsm;
	private SpriteRenderer _playerRenderer;

	private float _actionEffectDuration = 0.2f;

	private void Awake()
	{
		_playerRenderer = GetComponentInChildren<SpriteRenderer>();
		_fsm = GetComponent<PlayerFSM>();
	}

	public void PlayHitEffect()
	{
		_playerRenderer.color = new Color(0.5f, 0, 0, 1);
		_playerRenderer.DOColor(Color.white, _actionEffectDuration).SetEase(Ease.InCubic);
	}

	public void PlayDashEffect()
	{
		_dashParticle.transform.localPosition = new Vector3(
			_fsm.FacingDir.x switch
			{
				> 0 => -0.35f,
				< 0 => 0.35f,
				_ => 0
			}, 0.35f, 0);
		_dashParticle.Play();

		_playerRenderer.color = Color.skyBlue;
		_playerRenderer.DOColor(Color.white, _actionEffectDuration).SetEase(Ease.InCubic);

		_playerRenderer.transform.localScale = Vector3.one * 0.8f;
		_playerRenderer.transform.DOScale(Vector3.one, _actionEffectDuration).SetEase(Ease.InCubic);
	}

	public void PlaySkillExecutionEffect(int _)
	{
		_playerRenderer.color = Color.yellow;
		_playerRenderer.DOColor(Color.white, _actionEffectDuration).SetEase(Ease.InCubic);

		_playerRenderer.transform.localScale = Vector3.one * 0.9f;
		_playerRenderer.transform.DOScale(Vector3.one, _actionEffectDuration).SetEase(Ease.InCubic);
	}
}