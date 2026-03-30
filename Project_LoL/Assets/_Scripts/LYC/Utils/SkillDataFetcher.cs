#if UNITY_EDITOR
// ReSharper disable StringLiteralTypo
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Scripts.LYC.Utils
{
	public class SkillDataFetcher : EditorWindow
	{
		private string _sheetId = "1TkoTpA7iOtFlfnRQhcfno5GecN6i_1M_aG1LPYa44wk";
		private string _gid = "1203445732";
		private readonly int _dataStartRowIndex = 5;

		private const int IDIndex = 0;
		private const int NameIndex = 4;
		private const int DamageIndex = 5;
		private const int RangeXIndex = 6;
		private const int RangeYIndex = 7;
		private const int ProjectileSpeedIndex = 8;
		private const int RangeIndex = 9;
		private const int DelayIndex = 10;
		private const int MaxUseCountIndex = 11;
		private const int CooldownIndex = 12;
		private const int SkillPriceIndex = 13;
		private const int TypesIndex = 14;

		private bool _isRunning;
		private DataFetchingState _dataFetchingState = DataFetchingState.None;

		[MenuItem("Tools/Skill Data Fetcher")]
		public static void Open() => GetWindow<SkillDataFetcher>("SkillData Fetcher");

		private void OnGUI()
		{
			_sheetId = EditorGUILayout.TextField("Sheet ID", _sheetId);
			_gid = EditorGUILayout.TextField("  ID", _gid);

			if (_dataFetchingState is DataFetchingState.None or DataFetchingState.End)
			{
				if (GUILayout.Button("Fetch"))
				{
					_ = RunAsync();
				}
			}
			else
			{
				GUILayout.Label(_dataFetchingState.ToString());
			}
		}

		private async Task RunAsync()
		{
			_dataFetchingState = DataFetchingState.FetchingCSV;
			string rawCsv = await SheetFetcher.FetchCsvAsync(_sheetId, _gid);
			if (rawCsv == null)
			{
				this.LogError("데이터 테이블을 가져올 수 없음");
				_dataFetchingState = DataFetchingState.None;
				return;
			}

			this.Log($"CSV 수신 완료\n{rawCsv[..Mathf.Min(200, rawCsv.Length)]}...");

			_dataFetchingState = DataFetchingState.Parsing;
			var parsedData = Parse(rawCsv);
			if (parsedData == null || parsedData.Count == 0)
			{
				this.LogError("CSV Parsing Failed");
				_dataFetchingState = DataFetchingState.None;
				return;
			}

			_dataFetchingState = DataFetchingState.Applying;
			// await ApplyCSVToDataAsync(parsedData);

			_dataFetchingState = DataFetchingState.End;
		}

		private List<SkillRow> Parse(string csv)
		{
			List<SkillRow> value = new();
			string[] lines = csv.Split('\n');
			int lineIndex = _dataStartRowIndex;
			string line = "";
			string[] cols = new string[1];

			try
			{
				for (; lineIndex < lines.Length; lineIndex++)
				{
					line = lines[lineIndex].Trim();
					if (string.IsNullOrEmpty(line)) continue;

					cols = line.Split(',');
					if (cols.Length == 0 || string.IsNullOrEmpty(cols[0])) break;

					SkillRow skillRow = new()
					{
						SkillId = int.Parse(cols[IDIndex]),
						SkillText = cols[NameIndex],
						SkillDamage = int.Parse(cols[DamageIndex]),
						SkillRange = int.Parse(cols[RangeIndex]),
						SkillUseCount = int.Parse(cols[MaxUseCountIndex]),
						SkillCoolTime = int.Parse(cols[CooldownIndex]),
						SkillProjectileSpeed = int.Parse(cols[ProjectileSpeedIndex]),
						SkillDelay = float.Parse(cols[DelayIndex]),
						SkillDamageRangeX = int.Parse(cols[RangeXIndex]),
						SkillDamageRangeY = int.Parse(cols[RangeYIndex]),
						SkillPrice = int.Parse(cols[SkillPriceIndex]),
						PlayerSkillType = cols[TypesIndex]
					};

					value.Add(skillRow);
				}
			}
			catch (Exception e)
			{
				this.LogError($"{lineIndex}|{line} {e.Message}");
				return null;
			}

			return value;
		}

		private async Task<bool> ApplyCSVToDataAsync(List<SkillRow> data, string key)
		{
			await Addressables.LoadAssetAsync<SkillRow>(key).Task;

			// TODO (Reflection)

			return true;
		}
	}
#endif
}