using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataManager : MonoBehaviour
{
    [SerializeField] private List<SkillDataSO> _skillDataList;

    private Dictionary<string, SkillDataSO> _skillDataDictionary = new();

    public Action OnDataLoaded;
    public bool IsLoaded { get; private set; } = false;

    public void Init()
    {
        InitSkillDataDictionary();
    }

    private void InitSkillDataDictionary()
    {
        _skillDataDictionary.Clear();

        if (_skillDataList == null) return;

        foreach (var skill in _skillDataList)
        {
            if (skill == null) continue;

            string id = skill.name;

            if (!_skillDataDictionary.ContainsKey(id))
            {
                _skillDataDictionary.Add(id, skill);
            }
        }

        IsLoaded = true;
        OnDataLoaded?.Invoke();
        Debug.Log($"<color=cyan>스킬 데이터 {_skillDataDictionary.Count}개 준비 완료!</color>");
    }

    public List<SkillDataSO> GetRandomSkills(int count = 2)
    {
        if (!IsLoaded || _skillDataList.Count < count) return null;

        List<SkillDataSO> result = new List<SkillDataSO>();
        List<int> randomIndices = new List<int>();

        while (randomIndices.Count < count)
        {
            int randomIndex = UnityEngine.Random.Range(0, _skillDataList.Count);
            if (!randomIndices.Contains(randomIndex))
            {
                randomIndices.Add(randomIndex);
            }
        }

        foreach (int index in randomIndices)
        {
            result.Add(_skillDataList[index]);
        }

        return result;
    }
}