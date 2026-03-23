using System;
using TMPro;
using UnityEngine;

public class SamplePlayerUI : MonoBehaviour
{
	public PlayerController player;

	public TextMeshProUGUI statText;
	public TextMeshProUGUI stateText;

	private void Start()
	{
		player.died.AddListener(() => stateText.text = "Died");
		player.dashed.AddListener(() => stateText.text = "Dashed");
		player.hit.AddListener(() => stateText.text = "Hit");
		player.invincibilityChanged.AddListener(b => stateText.text = $"Set Invincible: {b}");
	}

	private void Update()
	{
		statText.text = $"hp: {player.Health}\n" +
		                $"exp: {player.Experience}\n" +
		                $"invincible: {(player.IsInvincible ? "O" : "X")}\n";
	}
}