[System.Serializable]
public class SkillDiceEffectGroup
{
	public System.Collections.Generic.List<DiceEffectSet> dice1Effects = new();
	public System.Collections.Generic.List<DiceEffectSet> dice2Effects = new();
	public System.Collections.Generic.List<DiceEffectSet> dice3Effects = new();
	public System.Collections.Generic.List<DiceEffectSet> dice4Effects = new();
	public System.Collections.Generic.List<DiceEffectSet> dice5Effects = new();
	public System.Collections.Generic.List<DiceEffectSet> dice6Effects = new();

	public System.Collections.Generic.List<DiceEffectSet> Get(int dice)
		=> dice switch
		{
			1 => dice1Effects,
			2 => dice2Effects,
			3 => dice3Effects,
			4 => dice4Effects,
			5 => dice5Effects,
			6 => dice6Effects,
			_ => null
		};
}