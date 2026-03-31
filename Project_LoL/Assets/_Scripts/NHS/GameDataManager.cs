using System;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    public DataManager equip => _equip;
    [SerializeField] private DataManager _equip;

    public SkillDataSO skillDataSO => _skillDataSO;
    [SerializeField] private SkillDataSO _skillDataSO;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            _equip.Init();

            _equip.LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
