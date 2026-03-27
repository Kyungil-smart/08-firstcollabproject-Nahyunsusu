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

    private void Awake()
    {
        InitEquipDataDictionary();
    }

    private void Start()
    {
        StartCoroutine(_sheet.Load(SetEquipDatas));
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
        for(int i=5;i<lines.Length;i++)
        {
            // 줄이 비어있으면 건너뛰기
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            // 나누는 문자열 기준(tsv인지 csv인지) 다시 문자열 배열로 쪼개서
            string[] cols = lines[i].Split(splitSymbol);

            // 컬럼 개수가 부족한 줄은 파싱하지 않음 (IndexOutOfRangeException 방지)
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
            Debug.Log($"{i}번 줄 파싱 시도: {cols[1]}");
            equip.SetData(cols);
        }
    }

    private void InitEquipDataDictionary()
    {
        _equipDataDictionary = _equipDataList.ToDictionary(data => data.name);
        //_equipDataList.Clear();
        //_equipDataList = null;
    }
}
