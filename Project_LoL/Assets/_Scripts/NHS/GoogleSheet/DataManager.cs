using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public SheetData _sheet;

    [SerializeField] private List<EquipmentData_SO> _equipDataList;

    private Dictionary<string, EquipmentData_SO> _equipDataDictionary = new();

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
        Debug.Log($"전체 라인 수: {lines.Length}");

        if (lines == null)
        {
            Debug.LogError("잘못된 데이터 입력");
            return;
        }

        // 모든 장비에 대해 수행해줘야함
        for (int i = 5; i < lines.Length; i++)
        {
            // 줄이 비어있으면 건너뛰기
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            // 나누는 문자열 기준(tsv인지 csv인지) 다시 문자열 배열로 쪼개서
            string[] cols = lines[i].Split(splitSymbol);

            // 컬럼 개수가 부족한 줄은 파싱하지 않음
            if (cols.Length < 12) continue;

            EquipmentData_SO equip;
            string equipName = cols[1].Trim();

            if (_equipDataDictionary.ContainsKey(equipName))
            {
                equip = _equipDataDictionary[equipName];
            }
            else
            {
                equip = ScriptableObject.CreateInstance<EquipmentData_SO>();
                equip.name = equipName;
                _equipDataDictionary.Add(equipName, equip);
                Debug.LogWarning($"<color=yellow>신규 장비 발견: {equipName}</color>");
            }
            //Debug.Log($"{i}번 줄 파싱 시도: {cols[1]}");
            equip.SetData(cols);
        }
        IsLoaded = true;
        Debug.Log("<color=green>데이터 로드 완료!</color>");
        OnDataLoaded?.Invoke();
    }

    private void InitEquipDataDictionary()
    {
        _equipDataDictionary = _equipDataList.ToDictionary(data => data.name);
        //_equipDataList.Clear();
        //_equipDataList = null;
    }

    public List<EquipmentData_SO> GetRandomEquips(int count = 2)
    {
        if (!IsLoaded || _equipDataList.Count < count) return null;

        List<EquipmentData_SO> result = new List<EquipmentData_SO>();
        HashSet<int> randomIndices = new HashSet<int>();

        while(randomIndices.Count < count)
        {
            int randomIndex = UnityEngine.Random.Range(0, _equipDataList.Count);
            randomIndices.Add(randomIndex);
        }

        foreach(int index in randomIndices)
        {
            result.Add(_equipDataList[index]);
        }

        return result;
    }
}