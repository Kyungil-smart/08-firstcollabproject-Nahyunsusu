using _Scripts.LYC.Data;

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
	{
		if (dice < DiceConst.MIN || dice > DiceConst.MAX)
		{
			UnityEngine.Debug.LogWarning($"[SkillDiceEffectGroup] 유효하지 않은 다이스 값: {dice} (범위: {DiceConst.MIN}~{DiceConst.MAX})");
			return null;
		}

		return dice switch
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
}