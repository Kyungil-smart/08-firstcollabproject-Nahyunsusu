using System;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, Damageable
{
	public PlayerSkillHandler handler;
	public event Action<DummyEnemy> OnHit;
	public ParticleSystem particle;
	public SpriteRenderer spriteRenderer;
	public int count = 0;
	public int targetIndex = 0;
	public int lastSkill = 0;

	private void Start()
	{
		handler.SkillExecuted.AddListener(SetLastSkillIndex);
		handler.SkillReloadFinished.AddListener(ChangeSkillIcon);
		handler.SkillChanged.AddListener(ChangeSkillIcon);
	}

	private void OnEnable()
	{
		count = 0;
	}

	private void OnDestroy()
	{
		handler.SkillExecuted.RemoveListener(SetLastSkillIndex);
		handler.SkillReloadFinished.RemoveListener(ChangeSkillIcon);
		handler.SkillChanged.RemoveListener(ChangeSkillIcon);
	}

	public void SetLastSkillIndex(int slot)
	{
		lastSkill = slot;
	}

	public void ChangeSkillIcon(int slot)
	{
		if (slot != targetIndex) return;

		spriteRenderer.sprite = handler.Skills[slot].CurrentSkillData.SkillImage;
	}

	public void TakeDamage(int damage)
	{
		if (lastSkill != targetIndex) return;

		count++;
		if (count == 5)
		{
			particle.Play();
			gameObject.SetActive(false);
			count = 0;
		}

		OnHit?.Invoke(this);
	}
}