using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private List<StoreItemSlot> _storeItems = new List<StoreItemSlot>(4);

    [SerializeField] private List<SkillDataSO>   _haveSkillList = new List<SkillDataSO>();
    [SerializeField] private List<Button>      _skillButtonList = new List<Button>();

    [SerializeField] private PlayerSkillHandler _skillHandler;

    private void Start()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.equip.OnDataLoaded += RefreshItem;
        }
    }

    private void OnEnable()
    {
        if(GameDataManager.instance != null && GameDataManager.instance.equip.IsLoaded)
        {
            RefreshItem();
        }
    }

    private void OnDestroy()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.equip.OnDataLoaded -= RefreshItem;
        }
    }

    public void RefreshItem()
    {
        if (this == null || gameObject == null) return;

        Debug.Log("상점 아이템 & 스킬 세팅 시작");

        if (GameDataManager.instance == null) return;

        var selectedEquips = GameDataManager.instance.equip.GetRandomEquips(2);

        if (selectedEquips == null || selectedEquips.Count >= 2)
        {
            _storeItems[0].SetItem(selectedEquips[0]);
            _storeItems[1].SetItem(selectedEquips[1]);
        }

        var skillManager = GameDataManager.instance.skillDataManager;
        var selectedSkills = skillManager.GetRandomSkills(2);

        if (selectedSkills != null && selectedSkills.Count >= 2)
        {
            _storeItems[2].SetSkill(selectedSkills[0], 1);
            _storeItems[3].SetSkill(selectedSkills[1], 2);
        }
        else
        {
            Debug.LogWarning("스킬 데이터를 뽑아오지 못했습니다. 리스트를 확인하세요.");
        }

        if (_haveSkillList == null) { _haveSkillList = new List<SkillDataSO>();}
        else { _haveSkillList.Clear(); }

        for (int i = 0; i < _skillHandler.Skills.Length; i++)
        {
            var executor = _skillHandler.Skills[i];

            if (executor != null && executor.SkillDataSO != null)
            {
                SkillDataSO currentSO = executor.SkillDataSO;
                _haveSkillList.Add(currentSO);

                if (i < _skillButtonList.Count && _skillButtonList[i] != null)
                {
                    _skillButtonList[i].gameObject.SetActive(true);
                    _skillButtonList[i].image.sprite = currentSO.Get(1).SkillImage;
                }
                Debug.Log($"{i}번 슬롯 스킬 세팅됨: {currentSO.name}");
            }
            else
            {
                if (i < _skillButtonList.Count && _skillButtonList[i] != null)
                    _skillButtonList[i].gameObject.SetActive(false);

                Debug.Log($"{i}번 슬롯 비어있음");
            }
        }
    }

}