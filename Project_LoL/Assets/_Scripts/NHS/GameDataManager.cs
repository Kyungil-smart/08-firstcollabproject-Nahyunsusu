using System;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    [SerializeField] private DataManager _equip;
    public DataManager equip => _equip;

    [SerializeField] private SkillDataSO _skillDataSO;
    public SkillDataSO skillDataSO => _skillDataSO;


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
