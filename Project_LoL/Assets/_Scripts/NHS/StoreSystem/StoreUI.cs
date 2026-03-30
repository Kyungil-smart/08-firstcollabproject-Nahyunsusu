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

    public void RefreshItem()
    {
        Debug.Log("RefreshItem 실행됨!");
        var selectedEquips = GameDataManager.instance.equip.GetRandomEquips(2);

        if (selectedEquips == null || selectedEquips.Count < _storeItems.Count)
        {
            Debug.LogWarning("아이템을 불러올 수 없습니다.");
            return;
        }

        for (int i = 0; i < _storeItems.Count; i++)
        {
            _storeItems[i].SetItem(selectedEquips[i]);
        }
    }

    public void RefreshSkill()
    {
        Debug.Log("RefreshSkill 실행됨!");

        var selectedSkills = GameDataManager.instance.equip.GetRandomEquips(2);

    }
}
