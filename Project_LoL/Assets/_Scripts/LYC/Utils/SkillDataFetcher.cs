#if UNITY_EDITOR
// ReSharper disable StringLiteralTypo
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
		private string _textGid = "105063179";
		private readonly int _dataStartRowIndex = 5;

		// PlayerSkillData 컬럼 인덱스
		private const int IDIndex = 0;
		private const int DescIndex = 1; // TextId
		private const int NameIndex = 2;
		private const int DamageIndex = 3;
		private const int RangeXIndex = 4;
		private const int RangeYIndex = 5;
		private const int ProjectileSpeedIndex = 6;
		private const int RangeIndex = 7;
		private const int DelayIndex = 8;
		private const int MaxUseCountIndex = 9;
		private const int CooldownIndex = 10;
		private const int SkillPriceIndex = 12;
		private const int TypesIndex = 13;

		// TextResource 컬럼 인덱스
		private const int TextIdIndex = 0;
		private const int TextKoreanIndex = 1;

		private DataFetchingState _dataFetchingState = DataFetchingState.None;

		[MenuItem("Tools/Skill Data Fetcher")]
		public static void Open() => GetWindow<SkillDataFetcher>("SkillData Fetcher");

		private void OnGUI()
		{
			_sheetId = EditorGUILayout.TextField("Sheet ID", _sheetId);
			_gid = EditorGUILayout.TextField("GID", _gid);
			_textGid = EditorGUILayout.TextField("Text GID", _textGid);

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

			string rawSkillCsv = await SheetFetcher.FetchCsvAsync(_sheetId, _gid);
			if (rawSkillCsv == null)
			{
				this.LogError("스킬 데이터 테이블을 가져올 수 없음");
				_dataFetchingState = DataFetchingState.None;
				return;
			}

			string rawTextCsv = await SheetFetcher.FetchCsvAsync(_sheetId, _textGid);
			if (rawTextCsv == null)
			{
				this.LogError("텍스트 리소스 테이블을 가져올 수 없음");
				_dataFetchingState = DataFetchingState.None;
				return;
			}

			this.Log($"CSV 수신 완료\n{rawSkillCsv[..Mathf.Min(200, rawSkillCsv.Length)]}...");

			_dataFetchingState = DataFetchingState.Parsing;

			var parsedData = ParseSkillData(rawSkillCsv);
			if (parsedData == null || parsedData.Count == 0)
			{
				this.LogError("스킬 CSV 파싱 실패");
				_dataFetchingState = DataFetchingState.None;
				return;
			}

			var textDict = ParseTextResource(rawTextCsv);
			if (textDict == null)
			{
				this.LogError("텍스트 리소스 파싱 실패");
				_dataFetchingState = DataFetchingState.None;
				return;
			}

			_dataFetchingState = DataFetchingState.Applying;
			await ApplyCSVToDataAsync(parsedData, textDict);

			_dataFetchingState = DataFetchingState.End;
		}

		private List<SkillRow> ParseSkillData(string csv)
		{
			List<SkillRow> value = new();
			string[] lines = csv.Split('\n');
			int lineIndex = _dataStartRowIndex;
			string line = "";

			try
			{
				for (; lineIndex < lines.Length; lineIndex++)
				{
					line = lines[lineIndex].Trim();
					if (string.IsNullOrEmpty(line)) continue;

					string[] cols = line.Split(',');
					if (cols.Length == 0 || string.IsNullOrEmpty(cols[0])) break;

					SkillRow skillRow = new()
					{
						SkillId = int.Parse(cols[IDIndex]),
						SkillDesc = cols[DescIndex],
						SkillText = cols[NameIndex],
						SkillDamage = int.Parse(cols[DamageIndex]),
						SkillDamageRangeX = int.Parse(cols[RangeXIndex]),
						SkillDamageRangeY = int.Parse(cols[RangeYIndex]),
						SkillProjectileSpeed = int.Parse(cols[ProjectileSpeedIndex]),
						SkillRange = int.Parse(cols[RangeIndex]),
						SkillDelay = float.Parse(cols[DelayIndex], CultureInfo.InvariantCulture),
						SkillUseCount = int.Parse(cols[MaxUseCountIndex]),
						SkillCoolTime = int.Parse(cols[CooldownIndex]),
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

		/// <summary>
		/// TextResource CSV(Google Sheets raw)를 파싱해 TextId → 한글 텍스트 딕셔너리로 반환합니다.
		/// 셀 내부에 쉼표·줄바꿈이 포함될 수 있으므로 quoted CSV 파서를 사용합니다.
		/// </summary>
		private Dictionary<int, string> ParseTextResource(string csv)
		{
			var rows = ParseQuotedCsv(csv);
			var dict = new Dictionary<int, string>();

			for (int i = _dataStartRowIndex; i < rows.Count; i++)
			{
				List<string> row = rows[i];
				if (row.Count == 0 || !int.TryParse(row[TextIdIndex].Trim(), out int id)) continue;
				dict[id] = row.Count > TextKoreanIndex ? row[TextKoreanIndex] : "";
			}

			this.Log($"TextResource 파싱 완료 ({dict.Count}개)");
			return dict;
		}

		/// <summary>
		/// 따옴표로 묶인 셀(쉼표·줄바꿈 포함)을 올바르게 처리하는 CSV 파서입니다.
		/// </summary>
		private static List<List<string>> ParseQuotedCsv(string raw)
		{
			var rows = new List<List<string>>();
			var currentRow = new List<string>();
			var cell = new StringBuilder();
			bool inQuotes = false;

			for (int i = 0; i < raw.Length; i++)
			{
				char c = raw[i];
				if (inQuotes)
				{
					if (c == '"' && i + 1 < raw.Length && raw[i + 1] == '"')
					{
						cell.Append('"');
						i++; // escaped quote ""
					}
					else if (c == '"')
					{
						inQuotes = false;
					}
					else
					{
						cell.Append(c);
					}
				}
				else
				{
					switch (c)
					{
						case '"':
							inQuotes = true;
							break;
						case ',':
							currentRow.Add(cell.ToString());
							cell.Clear();
							break;
						case '\n':
							currentRow.Add(cell.ToString());
							cell.Clear();
							rows.Add(currentRow);
							currentRow = new List<string>();
							break;
						case '\r':
							break;
						default:
							cell.Append(c);
							break;
					}
				}
			}

			if (currentRow.Count > 0 || cell.Length > 0)
			{
				currentRow.Add(cell.ToString());
				rows.Add(currentRow);
			}

			return rows;
		}

		private async Task ApplyCSVToDataAsync(List<SkillRow> data, Dictionary<int, string> textDict)
		{
			foreach (SkillRow row in data)
			{
				string key = $"s-{row.SkillId}";
				var handle = Addressables.LoadAssetAsync<SkillDataSO>(key);
				SkillDataSO so = await handle.Task;

				if (so == null)
				{
					this.LogError($"키 {key}에 해당하는 SkillDataSO를 찾을 수 없음");
					Addressables.Release(handle);
					continue;
				}

				string description = "";
				if (int.TryParse(row.SkillDesc, out int textId) && textDict.TryGetValue(textId, out string korean))
					description = korean;
				else
					this.LogError($"{key}: 텍스트 ID '{row.SkillDesc}'를 TextResource에서 찾을 수 없음");

				SerializedObject serializedObject = new SerializedObject(so);
				serializedObject.FindProperty("skillID").stringValue = row.SkillId.ToString();
				serializedObject.FindProperty("skillName").stringValue = row.SkillText;
				serializedObject.FindProperty("description").stringValue = description;
				serializedObject.FindProperty("skillType").intValue = (int)SkillFetcher.ConvertStringToSkillType(row.PlayerSkillType);
				serializedObject.FindProperty("damage").intValue = row.SkillDamage;
				serializedObject.FindProperty("damageRangeX").intValue = row.SkillDamageRangeX;
				serializedObject.FindProperty("damageRangeY").intValue = row.SkillDamageRangeY;
				serializedObject.FindProperty("projectileSpeed").floatValue = row.SkillProjectileSpeed;
				serializedObject.FindProperty("range").intValue = row.SkillRange;
				serializedObject.FindProperty("delay").floatValue = row.SkillDelay;
				serializedObject.FindProperty("maxUseCount").intValue = row.SkillUseCount;
				serializedObject.FindProperty("cooldown").intValue = row.SkillCoolTime;
				serializedObject.FindProperty("price").intValue = row.SkillPrice;
				serializedObject.ApplyModifiedProperties();

				EditorUtility.SetDirty(so);
				Addressables.Release(handle);

				this.Log($"{key} 적용 완료 ({row.SkillText})");
			}

			AssetDatabase.SaveAssets();
			this.Log("모든 SkillDataSO 저장 완료");
		}
	}
}
#endif
