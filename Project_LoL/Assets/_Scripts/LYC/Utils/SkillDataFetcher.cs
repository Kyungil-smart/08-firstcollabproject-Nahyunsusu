#if UNITY_EDITOR
// ReSharper disable StringLiteralTypo
using UnityEditor;
using UnityEngine;

public class SkillDataFetcher : EditorWindow
{
	private const int DataColumRowIndex = 1;
	private const int DataStartRowIndex = 4;

	private const string SkillID = "PlayerSkillId";
	// private const string SkillName = "PlayerSkillName";
	// private const string SkillID = "PlayerSkillId";
	// private const string SkillID = "PlayerSkillId";
	// private const string SkillID = "PlayerSkillId";
	// private const string SkillID = "PlayerSkillId";
	// private const string SkillID = "PlayerSkillId";
	
	private string _sheetId = "1TkoTpA7iOtFlfnRQhcfno5GecN6i_1M_aG1LPYa44wk";
	private string _skillStatGID = "1203445732";
	private string _skillBonusEffectGID = "292960602";

	private bool _isRunning;

	[MenuItem("Tools/Skill Data Fetcher")]
	public static void Open() => GetWindow<SkillDataFetcher>("Skill Data 동기화");

	private void OnGUI()
	{
		_sheetId = EditorGUILayout.TextField("Sheet ID", _sheetId);
		_skillStatGID = EditorGUILayout.TextField("Stat GID", _skillStatGID);
		_skillBonusEffectGID = EditorGUILayout.TextField("Bonus GID", _skillBonusEffectGID);

		if (_isRunning)
		{
		}

		if (GUILayout.Button("CSV 가져오기"))
		{
			_ = RunAsync();
		}
	}

	private async System.Threading.Tasks.Task RunAsync()
	{
		_isRunning = true;
		string statCsv = await SheetFetcher.FetchCsvAsync(_sheetId, _skillStatGID);
		if (statCsv == null)
		{
			Debug.LogError($"[{nameof(SkillDataFetcher)}] 스킬 스탯 기본 데이터 테이블을 가져올 수 없음");
			return;
		}

		OnStatCsvReceived(statCsv);

		string bonusCsv = await SheetFetcher.FetchCsvAsync(_sheetId, _skillBonusEffectGID);
		if (bonusCsv == null)
		{
			Debug.LogError($"[{nameof(SkillDataFetcher)}] 스킬 주사위 보너스 데이터 테이블을 가져올 수 없음");
			return;
		}

		OnBonusCsvReceived(bonusCsv);
	}

	private void OnStatCsvReceived(string csv)
	{
		Debug.Log($"CSV 수신 완료\n{csv[..Mathf.Min(200, csv.Length)]}...");
	}

	private void OnBonusCsvReceived(string csv)
	{
		Debug.Log($"CSV 수신 완료\n{csv[..Mathf.Min(200, csv.Length)]}...");
	}
}
#endif