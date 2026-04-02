using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public SheetData _sheet;

    public List<EquipmentData_SO> equipDataList => _equipDataList;
    [SerializeField] private List<EquipmentData_SO> _equipDataList;

    private Dictionary<int, EquipmentData_SO> _equipDataDictionary = new();

    public Action OnDataLoaded;

    public bool IsLoaded { get; private set; } = false;

    public void Init()
    {
        InitEquipDataDictionary();
    }

    public void LoadData()
    {
        Action<char, string[]> combinedCallback = SetEquipDatas;
        StartCoroutine(_sheet.Load(combinedCallback));
    }

    private void SetEquipDatas(char splitSymbol, string[] lines)
    {
        if (lines == null) return;

        for (int i = 5; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] cols = lines[i].Split(splitSymbol);

            if (cols.Length < 15) continue;

            if (!int.TryParse(cols[0].Trim(), out int EquipID))
            {
                Debug.LogWarning($"{i}번 줄: ID 파싱 실패");
                continue;
            }

            EquipmentData_SO equip;

            if (_equipDataDictionary.ContainsKey(EquipID))
            {
                equip = _equipDataDictionary[EquipID];
            }
            else
            {
                equip = ScriptableObject.CreateInstance<EquipmentData_SO>();
                equip.name = EquipID.ToString();
                _equipDataDictionary.Add(EquipID, equip);

                _equipDataList.Add(equip);

                Debug.LogWarning($"신규 장비 발견: {EquipID}");
            }

            equip.SetData(cols);
        }
        IsLoaded = true;
        OnDataLoaded?.Invoke();
    }

    private void InitEquipDataDictionary()
    {
        _equipDataDictionary = _equipDataList.ToDictionary(data => data.EquipID);
    }

    public enum EChanceType { Chance1, Chance2, Chance3 }

    public List<EquipmentData_SO> GetRandomEquips(int count = 2, EChanceType chanceType = EChanceType.Chance1)
    {
        if (!IsLoaded || _equipDataList.Count < count) return null;

        List<EquipmentData_SO> result = new List<EquipmentData_SO>();
        List<EquipmentData_SO> pool = new List<EquipmentData_SO>(_equipDataList);

        for (int i = 0; i < count; i++)
        {
            int totalWeight = pool.Sum(data => GetChanceByType(data, chanceType));

            if (totalWeight <= 0) break;

            int randomValue = UnityEngine.Random.Range(0, totalWeight);
            int currentSum = 0;

            for (int j = 0; j < pool.Count; j++)
            {
                currentSum += GetChanceByType(pool[j], chanceType);

                if (randomValue < currentSum)
                {
                    result.Add(pool[j]);
                    pool.RemoveAt(j);
                    break;
                }
            }
        }

        return result;
    }

    private int GetChanceByType(EquipmentData_SO data, EChanceType type)
    {
        return type switch
        {
            EChanceType.Chance1 => data.EquipChance1,
            EChanceType.Chance2 => data.EquipChance2,
            EChanceType.Chance3 => data.EquipChance3,
            _ => data.EquipChance1
        };
    }
}