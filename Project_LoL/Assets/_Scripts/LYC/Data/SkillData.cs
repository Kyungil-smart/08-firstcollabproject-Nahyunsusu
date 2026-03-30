[System.Serializable]
public class SkillData
{
	public string SkillID;
	public string SkillName;
	public SkillType SkillType;
	public UnityEngine.Sprite SkillImage;
	public string SkillDescription;

	public int Damage;
	public int DamageRangeX;
	public int DamageRangeY;
	public float ProjectileSpeed;
	public int Range;
	public float Delay;
	public int MaxUseCount;
	public int Cooldown;
	public int Price;
}