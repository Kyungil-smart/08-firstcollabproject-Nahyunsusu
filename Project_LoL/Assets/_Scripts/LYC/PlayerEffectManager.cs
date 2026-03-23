using DG.Tweening;
using UnityEngine;

public class PlayerEffectManager : MonoBehaviour
{
	private SpriteRenderer _renderer;

	private float _hitEffectDuration = 0.2f;

	private void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
	}

	public void PlayHitEffect()
	{
		_renderer.color = new Color(0.5f, 0, 0, 1);
		_renderer.DOColor(Color.white, _hitEffectDuration).SetEase(Ease.InCubic);
	}
}