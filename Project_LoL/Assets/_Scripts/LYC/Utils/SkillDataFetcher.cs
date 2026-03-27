#if UNITY_EDITOR
// ReSharper disable StringLiteralTypo
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SkillDataFetcher : EditorWindow
{
	private enum FetchState
	{
		None,
		FetchingCSV,
		ParsingStat,
		ApplyingStat,
		ParsingBonus,
		ApplyingBonus,
		End
	}

	private string _sheetId = "1TkoTpA7iOtFlfnRQhcfno5GecN6i_1M_aG1LPYa44wk";
	private string _skillStatGID = "1203445732";
	private string _skillBonusEffectGID = "292960602";
	private bool _isRunning;
	private FetchState _state = FetchState.None;

	[MenuItem("Tools/Skill Data Fetcher")]
	public static void Open() => GetWindow<SkillDataFetcher>("Skill Data 동기화");

	private void OnGUI()
	{
		_sheetId = EditorGUILayout.TextField("Sheet ID", _sheetId);
		_skillStatGID = EditorGUILayout.TextField("Stat GID", _skillStatGID);
		_skillBonusEffectGID = EditorGUILayout.TextField("Bonus GID", _skillBonusEffectGID);

		if (_isRunning)
		{
			GUILayout.TextArea(_state.ToString());
		}
		else if (GUILayout.Button("CSV Fetching"))
		{
			_ = RunAsync();
		}
	}

	private async Task RunAsync()
	{
		_isRunning = true;
		_state = FetchState.FetchingCSV;
		string statCsv = await SheetFetcher.FetchCsvAsync(_sheetId, _skillStatGID);
		if (statCsv == null)
		{
			Debug.LogError($"[{nameof(SkillDataFetcher)}] 스킬 스탯 기본 데이터 테이블을 가져올 수 없음");
			return;
		}

		await FetchAndApplyStat(statCsv);

		string bonusCsv = await SheetFetcher.FetchCsvAsync(_sheetId, _skillBonusEffectGID);
		if (bonusCsv == null)
		{
			Debug.LogError($"[{nameof(SkillDataFetcher)}] 스킬 주사위 보너스 데이터 테이블을 가져올 수 없음");
			return;
		}

		await FetchAndApplyBonus(bonusCsv);
	}

	private async Task FetchAndApplyStat(string csv)
	{
		Debug.Log($"Stat CSV 수신 완료\n{csv[..Mathf.Min(200, csv.Length)]}...");
		_state = FetchState.FetchingCSV;

		string[] lines = csv.Split('\n');

		// 0번째 행은 헤더 스킵
		for (int i = 1; i < lines.Length; i++)
		{
			string line = lines[i].Trim();
			if (string.IsNullOrEmpty(line)) continue;

			string[] cols = line.Split(',');
		}
	}

	private async Task FetchAndApplyBonus(string csv)
	{
		Debug.Log($"CSV 수신 완료\n{csv[..Mathf.Min(200, csv.Length)]}...");
	}

	private Type GetSkill(string raw)
	{
		switch (raw)
		{
			case "sc0001":
				return typeof(DiceRollSkill);
			case "sc0002":
				return typeof(BombsTimeSkill);
			case "sc0003":
				return typeof(DiceBuckSkill);
			case "sc0004":
				return typeof(ShapeDiceSkill);
			default:
				Debug.LogWarning($"[{typeof(SkillDataFetcher)}] skillId:{raw}는 아직 구현되지 않음");
				return null;
		}
	}

	private SkillType GetSkillType(string raw)
	{
		SkillType value = SkillType.None;
		string[] skillTypes = raw.Split(';').Select(s => s.Trim()).ToArray();
		foreach (var typeString in skillTypes)
		{
			if (Enum.TryParse(typeof(SkillType), typeString, out object result))
			{
				value |= (SkillType)result;
			}
			else
			{
				Debug.LogWarning($"[{typeof(SkillDataFetcher)}] {typeString}을 {nameof(SkillType)}내에서 찾을 수 없음");
			}
		}

		return value;
	}
}
#endif