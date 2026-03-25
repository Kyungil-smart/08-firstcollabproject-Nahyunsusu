[System.Serializable]
public class SkillDiceEffectGroup
{
	[System.Serializable]
	public class SkillDiceEffect
	{
		public SkillBonusType type;
		public float amount;
	}

	public System.Collections.Generic.List<SkillDiceEffect> dice1Effects = new();
	public System.Collections.Generic.List<SkillDiceEffect> dice2Effects = new();
	public System.Collections.Generic.List<SkillDiceEffect> dice3Effects = new();
	public System.Collections.Generic.List<SkillDiceEffect> dice4Effects = new();
	public System.Collections.Generic.List<SkillDiceEffect> dice5Effects = new();
	public System.Collections.Generic.List<SkillDiceEffect> dice6Effects = new();

	public System.Collections.Generic.List<SkillDiceEffect> Get(int dice)
		=> dice switch
		{
			1 => dice1Effects,
			2 => dice2Effects,
			3 => dice3Effects,
			4 => dice4Effects,
			5 => dice5Effects,
			_ => dice6Effects
		};
}