using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private List<StoreItemSlot> _storeItems = new List<StoreItemSlot>(4);

    public void DisplayStoreItem(List<Skill> skills)
    {
        for(int i=0;i<4;i+=2)
        {
            // TODO
            //texts[i+1] = skills.costs;
        }
    }

    public void DisplayHaveItem(List<Item> items)
    {

    }
}
