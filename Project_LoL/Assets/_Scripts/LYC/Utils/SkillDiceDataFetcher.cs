#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Scripts.LYC.Utils
{
	public class SkillDiceDataFetcher : EditorWindow
	{
		private string _sheetId = "1TkoTpA7iOtFlfnRQhcfno5GecN6i_1M_aG1LPYa44wk";
		private string _gid = "292960602";
		private readonly int _dataStartRowIndex = 5;

		private const int SkillIdIndex = 0;
		private const int PlayerSkillDiceIndex = 1;
		private const int PlayerSkillEffectTypeIndex = 2;
		private const int AmountIndex = 3;

		private DataFetchingState _state = DataFetchingState.None;

		[MenuItem("Tools/Skill Dice Data Fetcher")]
		public static void Open() => GetWindow<SkillDataFetcher>("SkillDiceData Fetcher");

		private void OnGUI()
		{
			_sheetId = EditorGUILayout.TextField("Sheet ID", _sheetId);
			_gid = EditorGUILayout.TextField("GID", _gid);

			if (_state is DataFetchingState.None or DataFetchingState.End)
			{
				if (GUILayout.Button("Fetch"))
				{
					_ = RunAsync();
				}
			}
			else
			{
				GUILayout.Label(_state.ToString());
			}
		}

		private async Task RunAsync()
		{
			_state = DataFetchingState.FetchingCSV;
			string rawCsv = await SheetFetcher.FetchCsvAsync(_sheetId, _gid);
			if (rawCsv == null)
			{
				this.LogError("데이터 테이블을 가져올 수 없음");
				_state = DataFetchingState.None;
				return;
			}

			this.Log($"CSV 수신 완료\n{rawCsv[..Mathf.Min(200, rawCsv.Length)]}...");

			_state = DataFetchingState.Parsing;
			var parsedData = Parse(rawCsv);
			if (parsedData == null || parsedData.Count == 0)
			{
				this.LogError("CSV Parsing Failed");
				_state = DataFetchingState.None;
				return;
			}

			_state = DataFetchingState.Applying;
			// await ApplyCSVToDataAsync(parsedData);

			_state = DataFetchingState.End;
		}

		private List<SkillDiceRow> Parse(string csv)
		{
			List<SkillDiceRow> value = new();
			string[] lines = csv.Split('\n');
			int lineIndex = _dataStartRowIndex;

			try
			{
				for (; lineIndex < lines.Length; lineIndex++)
				{
					string line = lines[lineIndex].Trim();
					if (string.IsNullOrEmpty(line)) continue;

					string[] cols = line.Split(',');
					if (cols.Length == 0) break;

					SkillDiceRow skillRow = new()
					{
						SkillId = int.Parse(cols[SkillIdIndex]),
						Amount = int.Parse(cols[AmountIndex]),
						SkillEffectType = cols[PlayerSkillEffectTypeIndex],
						PlayerSkillDice = int.Parse(cols[PlayerSkillDiceIndex])
					};

					value.Add(skillRow);
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"{lineIndex}:{e.Message}");
				return null;
			}

			return value;
		}

		private async Task<bool> ApplyCSVToDataAsync(List<SkillDiceRow> data, string key)
		{
			await Addressables.LoadAssetAsync<SkillDiceRow>(key).Task;

			// TODO (Reflection)

			return true;
		}
	}
}
#endif