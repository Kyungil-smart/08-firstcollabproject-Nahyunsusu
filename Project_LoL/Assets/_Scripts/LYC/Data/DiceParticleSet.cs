using UnityEngine;

[System.Serializable]
public class DiceParticleSet
{
	[Header("Dice 1")]
	public ParticleSystem particle1;

	[Header("Dice 2")]
	public ParticleSystem particle2;

	[Header("Dice 3")]
	public ParticleSystem particle3;

	[Header("Dice 4")]
	public ParticleSystem particle4;

	[Header("Dice 5")]
	public ParticleSystem particle5;

	[Header("Dice 6")]
	public ParticleSystem particle6;

	public ParticleSystem Get(int dice) => dice switch
	{
		1 => particle1,
		2 => particle2,
		3 => particle3,
		4 => particle4,
		5 => particle5,
		_ => particle6
	};
}