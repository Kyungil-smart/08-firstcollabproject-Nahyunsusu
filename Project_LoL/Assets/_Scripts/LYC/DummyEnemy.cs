using System;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, Damageable
{
	public PlayerSkillHandler handler;
	public event Action<DummyEnemy> OnHit;
	public ParticleSystem particle;
	public SpriteRenderer spriteRenderer;
	public int targetIndex = 0;

	private void Start()
	{
		handler.SkillReloadFinished.AddListener(ChangeSkillIcon);
		handler.SkillChanged.AddListener(ChangeSkillIcon);
	}

	private void OnDestroy()
	{
		handler.SkillReloadFinished.RemoveListener(ChangeSkillIcon);
		handler.SkillChanged.RemoveListener(ChangeSkillIcon);
	}

	public void ChangeSkillIcon(int slot)
	{
		if (slot != targetIndex) return;

		spriteRenderer.sprite = handler.Skills[slot].CurrentSkillData.SkillImage;
	}

	public void TakeDamage(int damage)
	{
		OnHit?.Invoke(this);
	}

	public void TakeDown()
	{
		particle.Play();
		gameObject.SetActive(false);
	}
}