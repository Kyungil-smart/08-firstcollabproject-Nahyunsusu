using System;
using UnityEngine;

public class ReplaceSelectorUI : MonoBehaviour
{
    public static ReplaceSelectorUI instance;

    public Action<int> onSlotSelected;

    private void Awake()
    {
        instance = this;
        this.gameObject.SetActive(false);
    }


    public void ClickSlot(int index)
    {
        onSlotSelected?.Invoke(index);

        gameObject.SetActive(false);
    }

    public void Open() => gameObject.SetActive(true);
}