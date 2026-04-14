#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
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
		public static void Open() => GetWindow<SkillDiceDataFetcher>("SkillDiceData Fetcher");

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
			await ApplyCSVToDataAsync(parsedData);

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
					if (cols.Length == 0 || string.IsNullOrEmpty(cols[0])) break;

					SkillDiceRow skillRow = new()
					{
						SkillId = int.Parse(cols[SkillIdIndex]),
						PlayerSkillDice = int.Parse(cols[PlayerSkillDiceIndex]),
						SkillEffectType = cols[PlayerSkillEffectTypeIndex],
						Amount = float.Parse(cols[AmountIndex], CultureInfo.InvariantCulture)
					};

					value.Add(skillRow);
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"{lineIndex}: {e.Message}");
				return null;
			}

			return value;
		}

		private async Task ApplyCSVToDataAsync(List<SkillDiceRow> data)
		{
			// SkillId 기준으로 그룹핑 (SO당 한 번만 로드)
			var grouped = new Dictionary<int, List<SkillDiceRow>>();
			foreach (SkillDiceRow row in data)
			{
				if (!grouped.ContainsKey(row.SkillId))
					grouped[row.SkillId] = new List<SkillDiceRow>();
				grouped[row.SkillId].Add(row);
			}

			foreach ((int skillId, List<SkillDiceRow> rows) in grouped)
			{
				string key = $"s-{skillId}";
				var handle = Addressables.LoadAssetAsync<SkillDataSO>(key);
				SkillDataSO so = await handle.Task;

				if (so == null)
				{
					this.LogError($"키 {key}에 해당하는 SkillDataSO를 찾을 수 없음");
					Addressables.Release(handle);
					continue;
				}

				SerializedObject serializedObject = new SerializedObject(so);
				SerializedProperty diceEffects = serializedObject.FindProperty("diceEffects");

				// 기존 주사위 효과 전체 초기화
				for (int d = 1; d <= 6; d++)
					diceEffects.FindPropertyRelative($"dice{d}Effects").ClearArray();

				// CSV 행을 순서대로 해당 주사위 슬롯에 추가
				foreach (SkillDiceRow row in rows)
				{
					DiceEffectSet effectSet = SkillFetcher.GetDiceEffectSet(row.SkillEffectType, row.Amount);
					if (effectSet == null) continue;

					SerializedProperty diceList = diceEffects.FindPropertyRelative($"dice{row.PlayerSkillDice}Effects");
					int idx = diceList.arraySize;
					diceList.InsertArrayElementAtIndex(idx);
					SerializedProperty element = diceList.GetArrayElementAtIndex(idx);
					element.FindPropertyRelative("type").intValue = (int)effectSet.type;
					element.FindPropertyRelative("amount").floatValue = effectSet.amount;
				}

				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(so);
				Addressables.Release(handle);

				this.Log($"{key} 주사위 효과 적용 완료");
			}

			AssetDatabase.SaveAssets();
			this.Log("모든 SkillDataSO 주사위 효과 저장 완료");
		}
	}
}
#endif