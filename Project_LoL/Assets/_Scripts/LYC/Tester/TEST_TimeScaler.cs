using UnityEngine;

namespace _Scripts.LYC.Tester
{
	public class TEST_TimeScaler : MonoBehaviour
	{
		[Range(0.01f, 2f)] public float timeMultiplier;

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