using System;
using UnityEngine;

namespace _Scripts.LYC.Tester
{
	public class TEST_TimeScaler : MonoBehaviour
	{
		[Range(0, 10)] public float timeMultiplier;

		private void Start()
		{
			Time.timeScale = timeMultiplier;
		}

		private void Update()
		{
			Time.timeScale = timeMultiplier;
			}
	}
}