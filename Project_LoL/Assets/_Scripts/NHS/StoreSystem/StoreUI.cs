using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private List<StoreItemSlot> _storeItems = new List<StoreItemSlot>(4);
    

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
        // 3. 메모리 누수 방지를 위해 이벤트 해제
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.equip.OnDataLoaded -= RefreshItem;
        }
    }

    private SkillData SetSkillDice(int num)
    {
        return GameDataManager.instance.skillDataSO.Get(num);
    }

    public void RefreshItem()
    {
        Debug.Log("상점 아이템 & 스킬 세팅 시작");

        var selectedEquips = GameDataManager.instance.equip.GetRandomEquips(2);

        var skillSO = GameDataManager.instance.skillDataSO;

        if (selectedEquips == null || selectedEquips.Count < 2)
        {
            Debug.LogWarning("장비 데이터가 부족하여 상점을 채울 수 없습니다.");
            return;
        }

        _storeItems[0].SetItem(selectedEquips[0]);
        _storeItems[1].SetItem(selectedEquips[1]);

        if (skillSO == null)
        {
            Debug.LogWarning("스킬 데이터이 부족하여 상점을 채울 수 없습니다.");
            return;
        }

        _storeItems[2].SetSkill(skillSO, 1); // 1번 주사위 효과 적용
        _storeItems[3].SetSkill(skillSO, 2); // 2번 주사위 효과 적용
    }

}
