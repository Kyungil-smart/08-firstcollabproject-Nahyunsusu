using System;
using TMPro;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, Damageable
{
	public event Action<DummyEnemy> OnHit;

	public PlayerSkillHandler handler;
	public Animator           animator;

	public ParticleSystem  particle;
	public SpriteRenderer  spriteRenderer;
	public TextMeshProUGUI damageUI;
	public int             targetIndex = 0;

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
		damageUI.SetText(damage.ToString());
		animator.SetTrigger("Hit");
		OnHit?.Invoke(this);
	}

	public void TakeDown()
	{
		particle.Play();
		damageUI.text = "";

		gameObject.SetActive(false);
	}
}