using UnityEngine;

namespace _Scripts.LYC.Skill
{
	public enum SkillExecuteResult
	{
		Success,
		NotExist,
		OnCooldown,
		Rolling
	}

	[System.Serializable]
	public class SkillExecutor
	{
		[field: SerializeField] public SkillData CurrentSkillData { get; private set; }
		[field: SerializeField] public SkillDataSO SkillDataSO { get; private set; }

		[field: SerializeField] public float LastExecutedTime { get; private set; }

		public Vector2 MouseDir => Controller.FSM.MouseDir;
		public Vector2 Position => Controller.transform.position;

		public PlayerController Controller { get; private set; }

		public int LastDiceResult
		{
			get => CurrentSkillData.CurrentDice;
			private set => CurrentSkillData.CurrentDice = value;
		}

		public int CurrentAmmo
		{
			get => CurrentSkillData.CurrentAmmo;
			private set => CurrentSkillData.CurrentAmmo = value;
		}

		public SkillExecutor(PlayerController controller)
		{
			Controller = controller;
			CurrentSkillData = null;
			SkillDataSO = null;
		}

		public void Set(SkillDataSO newSkill = null, int dice = 0)
		{
			SkillDataSO = newSkill;

			if (SkillDataSO != null)
			{
				RefreshData(dice);
				LastDiceResult = dice;
			}
		}

		private void RefreshData(int dice = 0)
		{
			if (dice == 0)
				dice = Roll();
			CurrentSkillData = SkillDataSO.Get(dice);
			CurrentAmmo = CurrentSkillData.MaxUseCount;
			LastDiceResult = dice;
		}

		public SkillExecuteResult TryExecute()
		{
			if (CurrentSkillData == null || SkillDataSO == null) return SkillExecuteResult.NotExist;
			if (LastExecutedTime + CurrentSkillData.Delay > Time.time) return SkillExecuteResult.OnCooldown;

			SkillDataSO.Use(this);
			CurrentAmmo--;
			LastExecutedTime = Time.time;

			if (CurrentSkillData.CurrentAmmo == 0)
			{
				RefreshData();
				return SkillExecuteResult.Rolling;
			}

			return SkillExecuteResult.Success;
		}

		public static int Roll(int min = 1, int max = 6)
			=> UnityEngine.Random.Range(min, max + 1);
	}
}