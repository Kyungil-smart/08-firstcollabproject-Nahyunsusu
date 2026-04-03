#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Scripts.LYC.Utils
{
	/// <summary>
	/// 구글 스프레드시트 CSV를 다운받아 LevelUpChoiceListSO에 적용하는 에디터 툴.
	///
	/// CSV 컬럼 (헤더 5행 스킵):
	///   0: ChoiceListId   (int)
	///   1: ChoiceName     (string)
	///   2: ChoiceText     (string, 미사용)
	///   3: ChoiceHp       (int)
	///   4: ChoiceAttackDamage (int)
	///   5: ChoiceAttackSpeed  (float)
	///   6: ChoiceMoveSpeed    (int)
	///   7: ChoiceCritChance   (int)
	///   8: ChoiceCritDamage   (int)
	///   9: Probability        (int) → spawnWeight
	/// </summary>
	public class LevelUpChoiceFetcher : EditorWindow
	{
		private string _sheetId = "1TkoTpA7iOtFlfnRQhcfno5GecN6i_1M_aG1LPYa44wk";
		private string _gid = "855959816";
		private readonly int _dataStartRowIndex = 5;

		// CSV 컬럼 인덱스
		private const int IdIndex = 0;

		private const int NameIndex = 1;

		// 2: ChoiceText — 사용 안 함
		private const int HpIndex = 3;
		private const int AtkDamageIndex = 4;
		private const int AtkSpeedIndex = 5;
		private const int MoveSpeedIndex = 6;
		private const int CritChanceIndex = 7;
		private const int CritDamageIndex = 8;
		private const int ProbabilityIndex = 9;

		// 아이콘 Addressables 키 (sprite 이름 "image" 고정)
		private static readonly (string key, Func<int, float, int, int, int, float> selector)[] IconCandidates =
		{
			("LvUp_AD[image]", (atk, _, __, ___, ____) => Mathf.Abs(atk)),
			("LvUp_AS[image]", (_, spd, __, ___, ____) => Mathf.Abs(spd)),
			("LvUp_Speed[image]", (_, __, mv, ___, ____) => Mathf.Abs(mv)),
			("LvUp_Critrate[image]", (_, __, ___, cc, ____) => Mathf.Abs(cc)),
			("LvUp_CritDmg[image]", (_, __, ___, ____, cd) => Mathf.Abs(cd)),
		};

		private LevelUpChoiceListSO _targetSO;
		private DataFetchingState _state = DataFetchingState.None;

		[MenuItem("Tools/LevelUp Choice Fetcher")]
		public static void Open() => GetWindow<LevelUpChoiceFetcher>("LevelUp Choice Fetcher");

		private void OnGUI()
		{
			_sheetId = EditorGUILayout.TextField("Sheet ID", _sheetId);
			_gid = EditorGUILayout.TextField("GID", _gid);
			_targetSO = (LevelUpChoiceListSO)EditorGUILayout.ObjectField(
				"Target SO", _targetSO, typeof(LevelUpChoiceListSO), false);

			EditorGUILayout.Space();

			if (_state is DataFetchingState.None or DataFetchingState.End)
			{
				EditorGUI.BeginDisabledGroup(_targetSO == null || string.IsNullOrEmpty(_sheetId));
				if (GUILayout.Button("Fetch & Apply"))
					_ = RunAsync();
				EditorGUI.EndDisabledGroup();
			}
			else
			{
				GUILayout.Label(_state.ToString());
			}
		}

		private async Task RunAsync()
		{
			_state = DataFetchingState.FetchingCSV;
			Repaint();

			string rawCsv = await SheetFetcher.FetchCsvAsync(_sheetId, _gid);
			if (rawCsv == null)
			{
				this.LogError("CSV 다운로드 실패");
				_state = DataFetchingState.None;
				Repaint();
				return;
			}

			this.Log($"CSV 수신 완료\n{rawCsv[..Mathf.Min(200, rawCsv.Length)]}...");

			_state = DataFetchingState.Parsing;
			Repaint();

			var rows = ParseCsv(rawCsv);
			if (rows == null || rows.Count == 0)
			{
				this.LogError("CSV 파싱 실패 또는 데이터 없음");
				_state = DataFetchingState.None;
				Repaint();
				return;
			}

			_state = DataFetchingState.Applying;
			Repaint();

			await ApplyToSOAsync(rows);

			_state = DataFetchingState.End;
			Repaint();
		}

		private List<string[]> ParseCsv(string csv)
		{
			var result = new List<string[]>();
			string[] lines = csv.Split('\n');

			for (int i = _dataStartRowIndex; i < lines.Length; i++)
			{
				string line = lines[i].Trim('\r', '\n', ' ');
				if (string.IsNullOrEmpty(line)) continue;

				string[] cols = line.Split(',');
				if (cols.Length == 0 || string.IsNullOrEmpty(cols[0])) break;

				result.Add(cols);
			}

			return result;
		}

		/// <summary>
		/// atkDamage, atkSpeed, moveSpeed, critChance, critDamage 중
		/// 절댓값이 가장 큰 스탯의 Addressables 키를 반환합니다. (HP는 아이콘 없음)
		/// </summary>
		private static string GetIconKey(int atkDamage, float atkSpeed, int moveSpeed, int critChance, int critDamage)
		{
			string bestKey = null;
			float bestAbs = 0f;

			foreach (var (key, selector) in IconCandidates)
			{
				float abs = selector(atkDamage, atkSpeed, moveSpeed, critChance, critDamage);
				if (abs > bestAbs)
				{
					bestAbs = abs;
					bestKey = key;
				}
			}

			return bestKey;
		}

		private async Task ApplyToSOAsync(List<string[]> rows)
		{
			// 1. 아이콘 스프라이트 전체 로드
			var handles = new Dictionary<string, AsyncOperationHandle<Sprite>>();
			var sprites = new Dictionary<string, Sprite>();

			foreach (var (key, _) in IconCandidates)
			{
				handles[key] = Addressables.LoadAssetAsync<Sprite>(key);
			}

			foreach (var (key, handle) in handles)
			{
				Sprite sprite = await handle.Task;
				if (sprite != null)
					sprites[key] = sprite;
				else
					this.LogError($"'{key}' 로드 실패 - 스프라이트가 null");
			}

			this.Log($"스프라이트 로드 완료 ({sprites.Count}개)");

			// 2. SO에 데이터 적용
			SerializedObject so = new SerializedObject(_targetSO);
			SerializedProperty entriesProp = so.FindProperty("entries");

			entriesProp.ClearArray();

			int appliedCount = 0;
			for (int i = 0; i < rows.Count; i++)
			{
				string[] cols = rows[i];

				try
				{
					if (!int.TryParse(cols[IdIndex].Trim(), out int id)) continue;

					string displayName = cols[NameIndex].Trim();
					int hp = int.Parse(cols[HpIndex].Trim());
					int atkDamage = int.Parse(cols[AtkDamageIndex].Trim());
					float atkSpeed = float.Parse(cols[AtkSpeedIndex].Trim(), CultureInfo.InvariantCulture);
					int moveSpeed = int.Parse(cols[MoveSpeedIndex].Trim());
					int critChance = int.Parse(cols[CritChanceIndex].Trim());
					int critDamage = int.Parse(cols[CritDamageIndex].Trim());
					int spawnWeight = int.Parse(cols[ProbabilityIndex].Trim());

					entriesProp.InsertArrayElementAtIndex(appliedCount);
					SerializedProperty element = entriesProp.GetArrayElementAtIndex(appliedCount);

					element.FindPropertyRelative("id").intValue = id;
					element.FindPropertyRelative("displayName").stringValue = displayName;
					element.FindPropertyRelative("hp").intValue = hp;
					element.FindPropertyRelative("atkDamage").intValue = atkDamage;
					element.FindPropertyRelative("atkSpeed").floatValue = atkSpeed;
					element.FindPropertyRelative("moveSpeed").intValue = moveSpeed;
					element.FindPropertyRelative("critChance").intValue = critChance;
					element.FindPropertyRelative("critDamage").intValue = critDamage;
					element.FindPropertyRelative("spawnWeight").intValue = spawnWeight;

					string iconKey = GetIconKey(atkDamage, atkSpeed, moveSpeed, critChance, critDamage);
					if (iconKey != null && sprites.TryGetValue(iconKey, out Sprite icon))
						element.FindPropertyRelative("icon").objectReferenceValue = icon;

					this.Log(
						$"Row {i}: ID={id} [{displayName}] hp={hp} atk={atkDamage} spd={atkSpeed} mv={moveSpeed} cc={critChance} cd={critDamage} w={spawnWeight}");
					appliedCount++;
				}
				catch (Exception e)
				{
					this.LogError($"Row {i} 파싱 오류: {e.Message}");
				}
			}

			so.ApplyModifiedProperties();
			EditorUtility.SetDirty(_targetSO);
			AssetDatabase.SaveAssets();

			this.Log($"LevelUpChoiceListSO 저장 완료 ({appliedCount}개 항목)");

			// 3. 핸들 전체 해제
			foreach (var handle in handles.Values)
				Addressables.Release(handle);

			this.Log("Addressables 핸들 전체 해제 완료");
		}
	}
}
#endif