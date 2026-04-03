using System;
using _Scripts.LYC.Data;
using UnityEngine;

namespace _Scripts.LYC.Skill
{
	public enum SkillExecuteResult
	{
		Success,
		NotExist,
		OnCooldown,
		Reload
	}

	[System.Serializable]
	public class SkillExecutor
	{
		[field: SerializeField] public SkillData CurrentSkillData { get; private set; }
		[field: SerializeField] public SkillDataSO SkillDataSO { get; private set; }

		[field: SerializeField] public float LastExecutedTime { get; private set; }
		[field: SerializeField] public bool IsRolling { get; private set; }

		public event Action ReloadFinished;

		private int _pendingReloadDice = -1;
		private float _reloadCooldown;

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
			// 리로딩 중이어도 새 스킬로 교체 시 pending 취소
			_pendingReloadDice = -1;
			IsRolling = false;

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
			CurrentSkillData.Delay /= (Controller.Data.AtkSpeed / 100.0f);
			CurrentAmmo = CurrentSkillData.MaxUseCount;
			LastDiceResult = dice;
		}

		public SkillExecuteResult TryExecute()
		{
			if (CurrentSkillData == null || SkillDataSO == null) return SkillExecuteResult.NotExist;
			if (LastExecutedTime + CurrentSkillData.Delay > Time.time) return SkillExecuteResult.OnCooldown;
			if (IsRolling && LastExecutedTime + _reloadCooldown > Time.time) return SkillExecuteResult.OnCooldown;

			SkillDataSO.Use(this);
			CurrentAmmo--;
			IsRolling = false;
			LastExecutedTime = Time.time;

			if (CurrentSkillData.CurrentAmmo == 0)
			{
				IsRolling = true;
				_pendingReloadDice = Roll();
				_reloadCooldown = CurrentSkillData.Cooldown;
				return SkillExecuteResult.Reload;
			}

			return SkillExecuteResult.Success;
		}

		// PlayerSkillHandler.Update에서 호출
		public void Tick(float currentTime)
		{
			if (!IsRolling || _pendingReloadDice < 0) return;
			if (currentTime < LastExecutedTime + _reloadCooldown) return;

			RefreshData(_pendingReloadDice);
			_pendingReloadDice = -1;
			IsRolling = false;
			ReloadFinished?.Invoke();
		}

		public static int Roll(int min = DiceConst.MIN, int max = DiceConst.MAX)
			=> UnityEngine.Random.Range(min, max + 1);
	}
}