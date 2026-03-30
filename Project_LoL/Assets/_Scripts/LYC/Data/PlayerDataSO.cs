// ReSharper disable IdentifierTypo

using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
	public PlayerData Get() =>
		new()
		{
			HP = PlayerHP,
			AtkDamage = PlayerAttack,
			AtkSpeed = PlayerAttackSpeed,
			CritDamage = PlayerCritDamage,
			CritRate = PlayerCritRate,
			DashCooldown = PlayerDashCooldown,
			DashInvincTime = PlayerDashInvincTime,
			DashTime = PlayerDashTime,
			MoveSpeed = PlayerMoveSpeed
		};

	[field: SerializeField] public int PlayerHP { get; private set; }
	[field: SerializeField] public int PlayerAttack { get; private set; }
	[field: SerializeField] public float PlayerAttackSpeed { get; private set; }
	[field: SerializeField] public int PlayerMoveSpeed { get; private set; }
	[field: SerializeField] public int PlayerCritRate { get; private set; }
	[field: SerializeField] public int PlayerCritDamage { get; private set; }
	[field: SerializeField] public float PlayerDashTime { get; private set; }
	[field: SerializeField] public float PlayerDashCooldown { get; private set; }
	[field: SerializeField] public float PlayerDashInvincTime { get; private set; }
}