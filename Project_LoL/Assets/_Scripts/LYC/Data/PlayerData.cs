public class PlayerData
{
	public float HP;
	public int AtkDamage;
	public int AtkSpeed;
	public float MoveSpeed;
	public float CritRate;
	public int CritDamage;
	public float DashTime;
	public float DashCooldown;
	public float DashInvincTime;

	[field: UnityEngine.SerializeField] public float CurrentHp { get; private set; }
	[field: UnityEngine.SerializeField] public int CurrentExp { get; private set; }
	[field: UnityEngine.SerializeField] public int CurrentLevel { get; private set; }
	[field: UnityEngine.SerializeField] public int CurrentGold { get; private set; }

	public void SetHp(float value)
	{
		CurrentHp = value;
	}

	public void SetExp(int value)
	{
		CurrentExp = value;
	}

	public void SetLevel(int value)
	{
		CurrentLevel = value;
	}

	public void SetGold(int value)
	{
		CurrentGold = value;
	}
}