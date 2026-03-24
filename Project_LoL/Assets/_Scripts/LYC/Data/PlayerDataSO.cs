// ReSharper disable IdentifierTypo

using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
	public PlayerData Get() =>
		new()
		{
			StatId = statId,
			MaxHp = maxHp,
			AtkDamage = atkDamage,
			AtkSpeed = atkSpeed,
			MoveSpeed = moveSpeed,
			CritRate = critRate,
			CritDamage = critDamage,
			DashTime = dashTime,
			DashCooldown = dashCooldown,
			DashInvincTime = dashInvincTime,
		};

	[SerializeField] private int statId;
	[SerializeField] private int maxHp;
	[SerializeField] private int atkDamage;
	[SerializeField] private float atkSpeed;
	[SerializeField] private int moveSpeed;
	[SerializeField] private int critRate;
	[SerializeField] private int critDamage;
	[SerializeField] private float dashTime;
	[SerializeField] private float dashCooldown;
	[SerializeField] private float dashInvincTime;
}