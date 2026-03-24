// ReSharper disable IdentifierTypo

using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
	public int StatId => statId;
	public int MaxHp => maxHp;
	public int AtkDamage => atkDamage;
	public float AtkSpeed => atkSpeed;
	public int MoveSpeed => moveSpeed;
	public int CritRate => critRate;
	public int CritDamage => critDamage;
	public float DashTime => dashTime;
	public float DashCooldown => dashCooldown;
	public float DashInvincTime => dashInvincTime;

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