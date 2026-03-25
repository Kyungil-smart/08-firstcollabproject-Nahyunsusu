[System.Serializable]
public class SkillData
{
	public string SkillID;
	public string SkillName;
	public AttackType SkillType;
	public UnityEngine.UI.Image SkillImage;
	public string SkillDescription;

	public int Damage;
	public int DamageRangeX;
	public int DamageRangeY;
	public float ProjectileSpeed;
	public int Range;
	public float Delay;
	public int MaxUseCount;
	public int Cooldown;
}