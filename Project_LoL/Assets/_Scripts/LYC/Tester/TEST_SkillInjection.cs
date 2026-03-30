using UnityEngine;

namespace _Scripts.LYC.Tester
{
	[System.Serializable]
	public class SKillInjectionData
	{
		public bool inject = true;
		public int slot;
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

			foreach (SKillInjectionData data in injectionArray)
			{
				if (data.inject)
					skillHandler.SetSkill(data.data, data.slot);
			}
		}
	}
}