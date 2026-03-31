using UnityEngine;

namespace _Scripts.LYC.Tester
{
	[System.Serializable]
	public class SKillInjectionData
	{
		public int dice = 0;
		public SkillDataSO data;
	}

	public class TEST_SkillInjection : MonoBehaviour
	{
		public PlayerSkillHandler skillHandler;
		public SKillInjectionData[] injectionArray;

		private void Start()
		{
			if (skillHandler == null) return;
			if (injectionArray == null) return;

			for (int i = 0; i < 4; i++)
			{
				var data = injectionArray[i];
				skillHandler.SetSkill(data.data, i, data.dice);
			}
		}
	}
}