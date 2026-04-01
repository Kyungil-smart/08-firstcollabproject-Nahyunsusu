using System;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    public DataManager equipDataManager => _equip;
    [SerializeField] private DataManager _equip;

    public SkillDataManager skillDataManager => _skillDataManager;
    [SerializeField] private SkillDataManager _skillDataManager;

    public EquipmentList equipItemList => _equipItemList;
    [SerializeField] private EquipmentList _equipItemList;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            _equip.Init();
            _skillDataManager.Init();

            _equip.LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
