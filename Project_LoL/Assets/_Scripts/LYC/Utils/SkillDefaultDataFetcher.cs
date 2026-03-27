#if UNITY_EDITOR
// ReSharper disable StringLiteralTypo
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Scripts.LYC.Skill;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkillDefaultDataFetcher : EditorWindow
{
    private enum State
    {
        None,
        FetchingCSV,
        Applying,
        Parsing,
        End
    }

    private class SkillRow
    {
        public string skillID, skillName;
        public int damage, damageRangeX, damageRangeY, range, maxUseCount, cooldown;
        public float projectileSpeed, delay;
    }

    private string _sheetId = "1TkoTpA7iOtFlfnRQhcfno5GecN6i_1M_aG1LPYa44wk";
    private string _skillStatGID = "1203445732";
    private int _dataStartRowIndex = 5;
    private int _dataColumnCount = 11;

    private const int IDIndex = 0;
    private const int NameIndex = 1;
    private const int DamageIndex = 2;
    private const int RangeXIndex = 3;
    private const int RangeYIndex = 4;
    private const int ProjectileSpeedIndex = 5;
    private const int RangeIndex = 6;
    private const int DelayIndex = 7;
    private const int MaxUseCountIndex = 8;
    private const int CooldownIndex = 9;
    private const int TypesIndex = 10;

    // private string _skillBonusEffectGID = "292960602";
    private bool _isRunning;
    private State _state = State.None;

    [MenuItem("Tools/Skill Data Fetcher")]
    public static void Open() => GetWindow<SkillDefaultDataFetcher>("SkillDefaultData 데이터 테이블 동기화");

    private void OnGUI()
    {
        _sheetId = EditorGUILayout.TextField("Sheet ID", _sheetId);
        _skillStatGID = EditorGUILayout.TextField("Stat GID", _skillStatGID);
        // _skillBonusEffectGID = EditorGUILayout.TextField("Bonus GID", _skillBonusEffectGID);

        if (_state is State.None or State.End)
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
        _state = State.FetchingCSV;
        string rawCsv = await SheetFetcher.FetchCsvAsync(_sheetId, _skillStatGID);
        if (rawCsv == null)
        {
            Debug.LogError($"[{nameof(SkillDefaultDataFetcher)}] 스킬 스탯 기본 데이터 테이블을 가져올 수 없음");
            _state = State.None;
            return;
        }

        Debug.Log($"Stat CSV 수신 완료\n{rawCsv[..Mathf.Min(200, rawCsv.Length)]}...");

        _state = State.Parsing;
        List<SkillRow> parsedData = Parse(rawCsv);
        if (parsedData == null || parsedData.Count == 0)
        {
            Debug.LogError($"[{nameof(SkillDefaultDataFetcher)}] CSV 파싱 오류");
            _state = State.None;
            return;
        }

        _state = State.Applying;
        // await ApplyCSVToDataAsync(parsedData);

        _state = State.End;
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
                if (cols.Length == 0) break;

                SkillRow skillRow = new()
                {
                    skillID = cols[IDIndex],
                    skillName = cols[NameIndex],
                    damage = int.Parse(cols[DamageIndex]),
                    range = int.Parse(cols[RangeIndex]),
                    maxUseCount = int.Parse(cols[MaxUseCountIndex]),
                    cooldown = int.Parse(cols[CooldownIndex]),
                    projectileSpeed = float.Parse(cols[ProjectileSpeedIndex]),
                    delay = float.Parse(cols[DelayIndex]),
                    damageRangeX = int.Parse(cols[RangeXIndex]),
                    damageRangeY = int.Parse(cols[RangeYIndex]),
                };

                value.Add(skillRow);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{lineIndex}|{line} {e.Message}");
            return null;
        }

        return value;
    }


    private async Task<bool> ApplyCSVToDataAsync(List<SkillRow> skillRow)
    {
        await Addressables.LoadAssetAsync<SkillDataSO>("temporaryKey").Task;
        return true;
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
                Debug.LogWarning($"[{typeof(SkillDefaultDataFetcher)}] skillId:{raw}는 아직 구현되지 않음");
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
                Debug.LogWarning($"[{typeof(SkillDefaultDataFetcher)}] {typeString}을 {nameof(SkillType)}내에서 찾을 수 없음");
            }
        }

        return value;
    }
}
#endif